using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public abstract class CastleState : IState
{
    protected Castle _castle;

    public CastleState(Castle castle)
    {
        _castle = castle;
    }

    public virtual void Enter() { }

    public virtual void ExecuteFixedUpdate() { }

    public virtual void ExecuteUpdate() { }

    public virtual void Exit() { }
}

public class CastleIdleState : CastleState
{
    public CastleIdleState(Castle unit) : base(unit) { }

    private Collider[] hitColliders = new Collider[20]; // 충돌을 저장할 배열
    private float _searchInterval = 0.2f; // 검색 주기 (0.2초)
    private float _lastSearchTime = 0f;
    private bool _targetChanged = false;
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void ExecuteFixedUpdate()
    {
        SearchEnemy();
    }

    private void SearchEnemy()
    {
        if (_castle.TargetEnemy != null)
        {
            if (_castle.TargetEnemy.layer == LayerMask.NameToLayer("DeadUnit"))
            {
                _castle.TargetEnemy = null;
                return;
            }

            float distance = Vector3.Distance(_castle.transform.position, _castle.TargetEnemy.transform.position);
            if (distance > _castle.AttackRadius) // 거리가 멀어질 때
            {
                _castle.TargetEnemy = null;
            }
            else if (distance < _castle.AttackRadius) // 공격 사거리 안으로 들어올 때 공격 상태로 진입
            {
                _castle.OnChangeState(new CastleAttackState(_castle));
            }
            return;
        }

        if (Time.time - _lastSearchTime < _searchInterval) return; // 검색 주기가 되지 않으면 반환
        _lastSearchTime = Time.time;

        Vector3 origin = _castle.transform.position;
        string[] targetLayers = _castle.IsTagAlly() ? new[] { "EnemyGroundUnit", "EnemyAirUnit" } : new[] { "AllyGroundUnit", "AllyAirUnit" };
        int layerMask = LayerMask.GetMask(targetLayers);

        int hitCount = Physics.OverlapSphereNonAlloc(origin, _castle.SearchRadius, hitColliders, layerMask);

        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            float distance = (_castle.transform.position - hitColliders[i].transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;

                if (_castle.TargetEnemy != hitColliders[i].gameObject) //타겟이 변경되었을 때
                {
                    _targetChanged = true;
                    _castle.TargetEnemy = hitColliders[i].gameObject;
                }

            }
        }
    }
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

public class CastleAttackState : CastleState
{
    public CastleAttackState(Castle unit) : base(unit) { }


    public override void Enter()
    {
        SetAnimationisAttack(true);
    }

    public override void ExecuteFixedUpdate()
    {
        CheckEnemy();
        Debug.Log("AtkFixed");
    }
    public override void Exit()
    {
        SetAnimationisAttack(false);
    }

    public void CheckEnemy()
    {
        if (_castle.TargetEnemy != null)
        {
            if (_castle.TargetEnemy.layer == LayerMask.NameToLayer("DeadUnit"))// 타겟 이너미가 죽었을 때
            {
                _castle.TargetEnemy = null;
                _castle.OnChangeState(new CastleIdleState(_castle));
                return;
            }

            float distance = Vector3.Distance(_castle.transform.position, _castle.TargetEnemy.transform.position);
            if (distance > _castle.AttackRadius + 0.1f) // 거리가 멀어질 때
            {
                _castle.TargetEnemy = null;
                _castle.OnChangeState(new CastleIdleState(_castle));
            }
            return;
        }
    }
    public void SetAnimationisAttack(bool isAttack)
    {
        _castle.Animator.SetBool("isAttack", isAttack);
    }
}

public class CastleTierUpState : CastleState
{
    public CastleTierUpState(Castle unit) : base(unit) { }

    public override void Enter()
    {
        _castle.CanAttack = false;
    }

    public override void ExecuteUpdate()
    {
        _castle.OnValueChanged_SpawnSlider(Time.deltaTime / _castle.SpawnTime);
    }

    public override void ExecuteFixedUpdate()
    {

    }
    public override void Exit()
    {
        _castle.CanAttack = true;
    }
}