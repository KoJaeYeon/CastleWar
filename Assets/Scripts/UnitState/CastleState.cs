using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

public class CastleIdleState : UnitState
{
    public CastleIdleState(Unit unit) : base(unit) { }

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
    }
}