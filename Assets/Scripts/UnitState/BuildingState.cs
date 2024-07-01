using UnityEngine;

public class BuildingIdleState : UnitState
{
    public BuildingIdleState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log("Entering BuildingIdle State");
        _unit.StartCoroutine(_unit.Spawn_Init());
        _unit.Helath += _unit.MaxHealth / 10;
    }

    public override void ExecuteUpdate()
    {
        _unit.OnValueChanged_SpawnSlider(Time.deltaTime / _unit.SpawnTime);
    }

    public override void ExecuteFixedUpdate()
    {
        _unit.Helath += Time.fixedDeltaTime * _unit.MaxHealth / _unit.SpawnTime;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
        _unit.Animator.SetTrigger("StartMove");

        // 타워나 건물이 후퇴영향 안받도록
        if (_unit.CanMove)
        {
            UnitManager.Instance.RegisterRetreatCallback(isTagAlly: _unit.IsTagAlly(), _unit.HandleOnRetreatState);
        }
        else
        {
            if (_unit.UnitId == -1)
            {
                CastleManager.Instance.AddCampToUnion(_unit.transform, _unit.IsTagAlly());
            }
        }
    }
}