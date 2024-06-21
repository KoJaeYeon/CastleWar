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
        _unit.OnValueChanged_SpawnSlider(Time.deltaTime);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
        _unit.Animator.SetTrigger("StartMove");
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

    public UnitMoveState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public override void ExecuteUpdate()
    {
    }

    public override void ExecuteFixedUpdate()
    {
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
        string[] targetLayers = _unit.tag.Equals("Ally") ? new[] { "EnemyGroundUnit", "EnemyAirUnit" } : new[] { "AllyGroundUnit", "AllyAirUnit" };
        int layerMask = LayerMask.GetMask(targetLayers);

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
       

    public override void Enter()
    {
        SetAnimationisAttack(true);
    }

    public override void ExecuteFixedUpdate()
    {
        CheckEnemy();
    }

    public override void ExecuteUpdate() { }

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

    public void SetAnimationisAttack(bool isAttack)
    {
        _unit.Animator.SetBool("isAttack", isAttack);
    }
}

public class UnitRetreatState : UnitState
{
    private List<Node> path; // A* 경로를 저장할 리스트
    private int currentPathIndex; // 현재 경로의 인덱스
    private Transform _castleTrans; // 복귀해야 할 캐슬의 트랜스폼
    private float _searchInterval = 0.2f; // 검색 주기 (0.2초)
    private float _lastSearchTime = 0f;
    private float _rotationSpeed = 3f;
    private bool _castleSearched = false;

    public UnitRetreatState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering Retreat State");

    }

    public override void ExecuteUpdate()
    {
    }

    public override void ExecuteFixedUpdate()
    {        
        if (!_castleSearched)
        {

        }
        else
        {
            if(SearchCastle()) // 캐슬과 가까워 졌을 때
            {
                if (BorderCheck())
                {
                    path = _unit.GetComponent<Astar>().AStar(_castleTrans.gameObject);
                    currentPathIndex = 0;

                    if (path == null || path.Count == 0)
                    {
                        _unit.TargetEnemy = null;
                        SearchCastle();
                    }
                }
                else
                {
                    path = null;
                }
            }
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
        Debug.Log("Exiting Retreat State");
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
    private bool SearchCastle()
    {
        if (Time.time - _lastSearchTime < _searchInterval) return false; // 검색 주기가 되지 않으면 반환
        _lastSearchTime = Time.time;

        Vector3 origin = _unit.transform.position;
        float distance = Vector3.Distance(_castleTrans.position, origin);

        if (distance < 15)
        {
            _castleSearched = true;

            return true;


        }
        else return false;

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

public class UnitDeadState : UnitState
{
    public UnitDeadState(Unit unit) : base(unit) { }


    public override void Enter()
    {
        _unit.gameObject.layer = LayerMask.NameToLayer("DeadUnit");
        _unit.Animator.SetTrigger("Death");
    }

    public override void ExecuteFixedUpdate()
    {
    }

    public override void ExecuteUpdate() { }

    public override void Exit()
    {

    }

}