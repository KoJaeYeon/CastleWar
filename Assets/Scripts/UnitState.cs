using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitState : IState
{
    protected Unit _character;

    public UnitState(Unit character)
    {
        _character = character;
    }

    public virtual void Enter() { }

    public virtual void ExecuteFixedUpdate() { }

    public virtual void ExecuteUpdate() { }

    public virtual void Exit() { }
}

public class UnitIdleState : UnitState
{
    public UnitIdleState(Unit character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        _character.StartCoroutine(_character.Spawn_Init());
    }

    public override void ExecuteUpdate()
    {
        _character.OnValueChanged_SpawnSlider(Time.deltaTime);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
        _character.OnCalledAnimationStartMove();
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

    public UnitMoveState(Unit character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public override void ExecuteUpdate()
    {
        SearchEnemy();
    }

    public override void ExecuteFixedUpdate()
    {
        if (_character.TargetChanged)
        {
            _character.TargetChanged = false;

            if (BorderCheck())
            {
                path = _character.GetComponent<Astar>().AStar(_character.TargetEnemy);
                currentPathIndex = 0;

                if (path == null || path.Count == 0)
                {
                    _character.TargetEnemy = null;
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
        else if (_character.TargetEnemy != null)
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
        if (_character.TargetEnemy != null)
        {
            Vector3 direction = (_character.TargetEnemy.transform.position - _character.transform.position).normalized;

            _character.transform.position += direction * _character.MoveSpeed * Time.fixedDeltaTime;

            PlayerRotateOnMove(direction);
        }
    }

    private void MoveNoneTarget()
    {
        Vector3 direction = _character.tag.Equals("Friend") ? Vector3.forward : Vector3.back;
        _character.transform.position += direction * _character.MoveSpeed * Time.fixedDeltaTime;

        PlayerRotateOnMove(direction);
    }

    private void MoveAlongPath()
    {
        Vector3 nextPosition = new Vector3(path[currentPathIndex].x, _character.transform.position.y, path[currentPathIndex].y);
        Vector3 direction = (nextPosition - _character.transform.position).normalized;
        _character.transform.position += direction * _character.MoveSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(_character.transform.position, nextPosition) < 0.1f)
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
        _character.transform.rotation = Quaternion.Slerp(_character.transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }
    #endregion
    private void SearchEnemy()
    {
        if (_character.TargetEnemy != null)
        {
            if (!_character.TargetEnemy.activeSelf)
            {
                _character.TargetEnemy = null;
                return;
            }

            float distance = Vector3.Distance(_character.transform.position, _character.TargetEnemy.transform.position);
            if (distance > _character.SearchRadius) // 거리가 멀어질 때
            {
                _character.TargetEnemy = null;
            }
            else if (distance < _character.AttackRadius) // 공격 사거리 안으로 들어올 때 공격 상태로 진입
            {
                _character.OnChangeState(new UnitAttackState(_character));
            }
            return;
        }

        if (Time.time - _lastSearchTime < _searchInterval) return; // 검색 주기가 되지 않으면 반환
        _lastSearchTime = Time.time;

        Vector3 origin = _character.transform.position;
        string[] targetLayers = _character.tag.Equals("Friend") ? new[] { "EnemyGroundUnit", "EnemyAirUnit" } : new[] { "FriendGroundUnit", "FriendAirUnit" };
        int layerMask = LayerMask.GetMask(targetLayers);

        int hitCount = Physics.OverlapSphereNonAlloc(origin, _character.SearchRadius, hitColliders, layerMask);

        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (!hitColliders[i].CompareTag(_character.tag))
            {
                float distance = (_character.transform.position - hitColliders[i].transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _character.TargetEnemy = hitColliders[i].gameObject;
                    _character.TargetChanged = true;
                }
            }
        }

        //if (_character.TargetEnemy != null)
        //{
        //    Debug.Log("Target enemy: " + _character.TargetEnemy.name);
        //}
    }

    private bool BorderCheck()
    {
        if (_character.TargetEnemy == null)
        {
            return false;
        }

        Vector3 direction = (_character.TargetEnemy.transform.position - _character.transform.position).normalized;
        float distance = Vector3.Distance(_character.transform.position, _character.TargetEnemy.transform.position);
        int layerMask = LayerMask.GetMask("Border");

        if (Physics.Raycast(_character.transform.position, direction, out RaycastHit hit, distance, layerMask))
        {
            return true;
        }

        return false;
    }
}
public class UnitAttackState : UnitState
{
    public UnitAttackState(Unit character) : base(character) { }

    public override void Enter()
    {
        _character.OnCalledAnimationisAttack(true);
    }

    public override void ExecuteFixedUpdate()
    {
        CheckEnemy();
    }

    public override void ExecuteUpdate() { }

    public override void Exit()
    {
        _character.OnCalledAnimationisAttack(false);
    }

    public void CheckEnemy()
    {
        if (_character.TargetEnemy != null)
        {
            if (!_character.TargetEnemy.activeSelf) // 타겟 이너미가 죽었을 때
            {
                _character.TargetEnemy = null;
                _character.OnChangeState(new UnitMoveState(_character));
                return;
            }

            float distance = Vector3.Distance(_character.transform.position, _character.TargetEnemy.transform.position);
            if (distance > _character.AttackRadius) // 거리가 멀어질 때
            {
                _character.TargetEnemy = null;
                _character.OnChangeState(new UnitMoveState(_character));
            }
            return;
        }
    }
}