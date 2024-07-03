using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitState : IState
{
    protected Unit _unit;

    public UnitState(Unit unit)
    {
        _unit = unit;
    }

    public virtual void Enter() { }

    public virtual void ExecuteFixedUpdate() { }

    public virtual void ExecuteUpdate() { }

    public virtual void Exit() { }
}

public class UnitIdleState : UnitState
{
    public UnitIdleState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        _unit.StartCoroutine(_unit.Spawn_Init());
    }

    public override void ExecuteUpdate()
    {
        _unit.OnValueChanged_SpawnSlider(Time.deltaTime / _unit.SpawnTime);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
        _unit.Animator.SetTrigger("StartMove");

        UnitManager.Instance.RegisterRetreatCallback(isTagAlly: _unit.IsTagAlly(), _unit.HandleOnRetreatState);
    }
}

public class UnitMoveState : UnitState
{
    private List<Node> path; // A* 경로를 저장할 리스트
    private int currentPathIndex; // 현재 경로의 인덱스
    private Collider[] hitColliders = new Collider[10]; // 충돌을 저장할 배열
    private float _searchInterval = 0.2f; // 검색 주기 (0.2초)
    private float _lastSearchTime = 0f;
    private float _rotationSpeed = 3f;
    private bool _targetChanged = false;

    int layerMask;
    public UnitMoveState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
        string[] targetLayers = GetTargetLayers();
        layerMask = LayerMask.GetMask(targetLayers);
    }

    string[] GetTargetLayers()
    {
        switch (_unit.AttackType)
        {
            case UnitType.Ground:
                return _unit.IsTagAlly() ? new[] { "EnemyGroundUnit", "EnemyBuilding" } : new[] { "AllyGroundUnit", "AllyBuilding" };
            case UnitType.Air:
                return _unit.IsTagAlly() ? new[] { "EnemyAirUnit" } : new[] { "AllyAirUnit" };
            case UnitType.Both:
                return _unit.IsTagAlly() ? new[] { "EnemyGroundUnit", "EnemyAirUnit", "EnemyBuilding" } : new[] { "AllyGroundUnit", "AllyAirUnit", "AllyBuilding" };
            case UnitType.Building:
                return _unit.IsTagAlly() ? new[] { "EnemyBuilding" } : new[] { "AllyBuilding" };
            default:
                return null;

        }
    }

    public override void ExecuteFixedUpdate()
    {
        if (!_unit.CanAttack) return;
        SearchEnemy();
        if (_targetChanged)
        {
            _targetChanged = false;

            if (BorderCheck())
            {
                path = _unit.GetComponent<Astar>().AStar(_unit.TargetEnemy);
                currentPathIndex = 0;

                if (path == null || path.Count == 0)
                {
                    _unit.TargetEnemy = null;
                    SearchEnemy();
                }
            }
            else
            {
                path = null;
            }
        }

        if(!_unit.CanMove)
        {
            return;
        }

        if (path != null && currentPathIndex < path.Count)
        {
            MoveAlongPath();
        }
        else if (_unit.TargetEnemy != null)
        {
            MoveTowardsTarget();
        }
        else
        {
            MoveNoneTarget();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Move State");
    }

    #region Move
    private void MoveTowardsTarget()
    {
        if (_unit.TargetEnemy != null)
        {
            Vector3 direction = (_unit.TargetEnemy.transform.position - _unit.transform.position).normalized;

            _unit.transform.position += direction * _unit.MoveSpeed * Time.fixedDeltaTime;

            PlayerRotateOnMove(direction);
        }
    }

    private void MoveNoneTarget()
    {
        Vector3 direction;
        if (_unit.MapCornerPoint == MapCornerPoint.NoCorner)
        {
            direction = _unit.tag.Equals("Ally") ? Vector3.forward : Vector3.back;
        }
        else
        {
            direction = CheckMapCorner();
        }
        _unit.transform.position += direction * _unit.MoveSpeed * Time.fixedDeltaTime;

        PlayerRotateOnMove(direction);
    
    
    }

    private Vector3 CheckMapCorner()
    {
        if (_unit.tag == "Ally")
        {
            return CheckMapCorner_GoUp();
        }
        else
        {
            return CheckMapCorner_GoDown();
        }
    }
    private Vector3 CheckMapCorner_GoUp()
    {
        switch (_unit.MapCornerPoint)
        {
            case MapCornerPoint.BottomLeftCenter:
            case MapCornerPoint.TopRight:
            case MapCornerPoint.TopRightCenter:
                return Vector3.left;
            case MapCornerPoint.BottomRightCenter:
            case MapCornerPoint.TopLeft:
            case MapCornerPoint.TopLeftCenter:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }

    private Vector3 CheckMapCorner_GoDown()
    {
        switch (_unit.MapCornerPoint)
        {
            case MapCornerPoint.BottomRightCenter:
            case MapCornerPoint.BottomRight:
            case MapCornerPoint.TopLeftCenter:
                return Vector3.left;
            case MapCornerPoint.BottomLeftCenter:
            case MapCornerPoint.BottomLeft:
            case MapCornerPoint.TopRightCenter:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }

    private void MoveAlongPath()
    {
        Vector3 nextPosition = new Vector3(path[currentPathIndex].x, _unit.transform.position.y, path[currentPathIndex].y);
        Vector3 direction = (nextPosition - _unit.transform.position).normalized;
        _unit.transform.position += direction * _unit.MoveSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(_unit.transform.position, nextPosition) < 0.1f)
        {
            currentPathIndex++;

            if (currentPathIndex < path.Count && !BorderCheck())
            {
                path.Clear();
                return;
            }
        }

        PlayerRotateOnMove(direction);
    }
    #endregion
    #region Roation
    public void PlayerRotateOnMove(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }
    #endregion
    private void SearchEnemy()
    {
        if (_unit.TargetEnemy != null)
        {
            if (_unit.TargetEnemy.layer == LayerMask.NameToLayer("DeadUnit"))
            {
                _unit.TargetEnemy = null;
                return;
            }

            float distance = Vector3.Distance(_unit.transform.position, _unit.TargetEnemy.transform.position);
            if (distance > _unit.SearchRadius) // 거리가 멀어질 때
            {
                _unit.TargetEnemy = null;
            }
            else if (distance < _unit.AttackRadius) // 공격 사거리 안으로 들어올 때 공격 상태로 진입
            {
                _unit.OnChangeState(new UnitAttackState(_unit));
            }
            return;
        }

        if (Time.time - _lastSearchTime < _searchInterval) return; // 검색 주기가 되지 않으면 반환
        _lastSearchTime = Time.time;

        Vector3 origin = _unit.transform.position;

        int hitCount = Physics.OverlapSphereNonAlloc(origin, _unit.SearchRadius, hitColliders, layerMask);

        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (!hitColliders[i].CompareTag(_unit.tag))
            {
                float distance = (_unit.transform.position - hitColliders[i].transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;

                    if(_unit.TargetEnemy != hitColliders[i].gameObject) //타겟이 변경되었을 때
                    {
                        _targetChanged = true;
                        _unit.TargetEnemy = hitColliders[i].gameObject;
                    }                   
                    
                }
            }
        }

        //if (_unit.TargetEnemy != null)
        //{
        //    Debug.Log("Target enemy: " + _unit.TargetEnemy.name);
        //}
    }

    private bool BorderCheck()
    {
        if (_unit.TargetEnemy == null)
        {
            return false;
        }

        Vector3 direction = (_unit.TargetEnemy.transform.position - _unit.transform.position).normalized;
        float distance = Vector3.Distance(_unit.transform.position, _unit.TargetEnemy.transform.position);
        int layerMask = LayerMask.GetMask("Border");

        if (Physics.Raycast(_unit.transform.position, direction, out RaycastHit hit, distance, layerMask))
        {
            return true;
        }

        return false;
    }
}


public class UnitAttackState : UnitState
{
    public UnitAttackState(Unit unit) : base(unit) { }
    float _rotationSpeed = 3f;

    public override void Enter()
    {
        SetAnimationisAttack(true);
    }

    public override void ExecuteFixedUpdate()
    {
        CheckEnemy();
        if(!_unit.CanAttack)
        {
            _unit.OnChangeState(new UnitMoveState(_unit));
        }
        if(_unit.TargetEnemy != null)
        {
            Vector3 direction = (_unit.TargetEnemy.transform.position - _unit.transform.position).normalized;
            UnitRotateOnAttack(direction);
        }


    }
    public override void Exit()
    {
        SetAnimationisAttack(false);


    }

    public void CheckEnemy()
    {
        if (_unit.TargetEnemy != null)
        {
            if (_unit.TargetEnemy.layer == LayerMask.NameToLayer("DeadUnit"))// 타겟 이너미가 죽었을 때
            {
                _unit.TargetEnemy = null;
                _unit.OnChangeState(new UnitMoveState(_unit));
                return;
            }

            float distance = Vector3.Distance(_unit.transform.position, _unit.TargetEnemy.transform.position);
            if (distance > _unit.AttackRadius + 0.1f) // 거리가 멀어질 때
            {
                _unit.TargetEnemy = null;
                _unit.OnChangeState(new UnitMoveState(_unit));
            }
            return;
        }
    }

    public void UnitRotateOnAttack(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }

    public void SetAnimationisAttack(bool isAttack)
    {
        _unit.Animator.SetBool("isAttack", isAttack);
    }
}

public class UnitRetreatState : UnitState
{
    private float _searchInterval = 0.2f; // 검색 주기 (0.2초)
    private float _lastSearchTime = 0f;
    private Transform _castleTrans;
    private float _rotationSpeed = 3f;
    private bool _castleSearched = false;
    private float _retreatDistance = 21f;

    public UnitRetreatState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering Retreat State");
        GameObject castle = CastleManager.Instance.GetCastleGameObj(_unit.IsTagAlly());
        _castleTrans = castle.transform;
        _castleSearched =false;
        UnitManager.Instance.RegisterCancelCallback(isTagAlly:_unit.IsTagAlly(), _unit.HandleOnMoveState);
    }
    public override void ExecuteFixedUpdate()
    {
        if(_castleSearched)
        {
            MoveTowardsTarget_Retreat();
            ReturnCastle(); // 가까이가면 회수되는 함수
        }
        else
        {
            MoveNoneTarget_Retreat();
            SearchCastle();
        }
        
    }

    public override void Exit()
    {
        Debug.Log("Exiting Retreat State");
        UnitManager.Instance.UnRegisterCancelCallback(_unit.IsTagAlly(),_unit.HandleOnMoveState);
    }

    #region Move

    private void SearchCastle()
    {
        if (Time.time - _lastSearchTime < _searchInterval) return; // 검색 주기가 되지 않으면 반환
        _lastSearchTime = Time.time;

        Vector3 origin = _unit.transform.position;

        float distance = Vector3.Distance(_castleTrans.position, origin);

        if(distance < _retreatDistance)
        {
            _castleSearched=true;
        }
    }

    private void ReturnCastle()
    {
        Vector3 origin = _unit.transform.position;

        float distance = Vector3.Distance(_castleTrans.position, origin);

        Debug.Log(distance);

        if (distance < 8f)
        {
            {
                //귀환 마나 적용
                GameManager.Instance.RequestManaProduce((int)(_unit.Cost * 0.3f));
                GameManager.Instance.RequestPopulationUse(_unit.Population * -1);
            }

            SpawnManager.Instance.OnCalled_ReturnUnit(_unit.SpwanSlotIndex, _unit.gameObject);
            Exit();
            UnitManager.Instance.UnRegisterRetreatCallback(isTagAlly:_unit.IsTagAlly(), _unit.HandleOnRetreatState);
        }
    }

    private void MoveTowardsTarget_Retreat()
    {

        Vector3 direction = (_castleTrans.position - _unit.transform.position).normalized;

        _unit.transform.position += direction * _unit.MoveSpeed * Time.fixedDeltaTime;

        PlayerRotateOnMove(direction);

    }

    private void MoveNoneTarget_Retreat()
    {
        Vector3 direction;
        if (_unit.MapCornerPoint == MapCornerPoint.NoCorner)
        {
            direction = _unit.tag.Equals("Ally") ? Vector3.back : Vector3.forward;
        }
        else
        {
            direction = CheckMapCorner();
        }
        _unit.transform.position += direction * _unit.MoveSpeed * Time.fixedDeltaTime;

        PlayerRotateOnMove(direction);
    }

    private Vector3 CheckMapCorner()
    {
        if (_unit.IsTagAlly())
        {
            return CheckMapCorner_GoDown();
        }
        else
        {
            return CheckMapCorner_GoUp();
        }
    }
    private Vector3 CheckMapCorner_GoUp()
    {
        switch (_unit.MapCornerPoint)
        {
            case MapCornerPoint.BottomLeftCenter:
            case MapCornerPoint.TopRight:
            case MapCornerPoint.TopRightCenter:
                return Vector3.left;
            case MapCornerPoint.BottomRightCenter:
            case MapCornerPoint.TopLeft:
            case MapCornerPoint.TopLeftCenter:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }

    private Vector3 CheckMapCorner_GoDown()
    {
        switch (_unit.MapCornerPoint)
        {
            case MapCornerPoint.BottomRightCenter:
            case MapCornerPoint.BottomRight:
            case MapCornerPoint.TopLeftCenter:
                return Vector3.left;
            case MapCornerPoint.BottomLeftCenter:
            case MapCornerPoint.BottomLeft:
            case MapCornerPoint.TopRightCenter:
                return Vector3.right;
            default:
                return Vector3.back;
        }
    }
    #endregion
    #region Roation
    public void PlayerRotateOnMove(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }
    #endregion
}

public class UnitDeadState : UnitState
{
    public UnitDeadState(Unit unit) : base(unit) { }


    public override void Enter()
    {
        _unit.gameObject.layer = LayerMask.NameToLayer("DeadUnit");
        _unit.Animator.SetTrigger("Death");
        UnitManager.Instance.UnRegisterRetreatCallback(_unit.IsTagAlly(), _unit.HandleOnRetreatState);
    }

    public override void Exit()
    {
        SpawnManager.Instance.OnCalled_ReturnUnit(_unit.SpwanSlotIndex, _unit.gameObject);
    }
}