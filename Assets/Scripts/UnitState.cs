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
    }
}

public class UnitMoveState : UnitState
{
    private List<Node> path; // A* 경로를 저장할 리스트
    private int currentPathIndex; // 현재 경로의 인덱스
    private Collider[] hitColliders = new Collider[10]; // 충돌을 저장할 배열
    private float _searchInterval = 0.2f; // 검색 주기 (0.2초)
    private float _lastSearchTime = 0f;

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
            Move();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Move State");
    }

    private void MoveTowardsTarget()
    {
        if (_character.TargetEnemy != null)
        {
            Vector3 direction = (_character.TargetEnemy.transform.position - _character.transform.position).normalized;
            _character.transform.position += direction * _character.MoveSpeed * Time.fixedDeltaTime;
        }
    }

    private void Move()
    {
        Vector3 direction = _character.transform.forward;
        _character.transform.position += direction * _character.MoveSpeed * Time.fixedDeltaTime;
    }

    private void MoveAlongPath()
    {
        if (path != null && currentPathIndex < path.Count)
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
                    MoveTowardsTarget();
                    return;
                }
            }
        }
    }

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
            if (distance > _character.SearchRadius) _character.TargetEnemy = null;
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

        if (_character.TargetEnemy != null)
        {
            Debug.Log("Target enemy: " + _character.TargetEnemy.name);
        }
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
            Debug.Log("Hit Border: " + hit.collider.name);
            return true;
        }

        return false;
    }
}
