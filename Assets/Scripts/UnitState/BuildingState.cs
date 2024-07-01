﻿using UnityEngine;

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
        Debug.Log("Exiting BuildingIdle State");
        if (_unit.UnitId == -1)
        {
            CastleManager.Instance.AddCampToUnion(_unit.transform, _unit.IsTagAlly());
        }
        else if (_unit.UnitId == -2)
        {
            CastleManager.Instance.AddSancToUnion(_unit.transform, _unit.IsTagAlly());
        }
        else
        {
            _unit.Animator.SetTrigger("StartMove");
        }

    }
}

public class BuildingProduceState : UnitState
{
    public BuildingProduceState(Unit unit) : base(unit) { }
    float _elapsedTime;
    float _produceTime;

    Coroutine coroutine;
    public override void Enter()
    {
        Debug.Log("Entering BuildingProduce State");
        if (_unit.UnitId == -2)
        {
            coroutine = _unit.StartCoroutine(_unit.ProduceMana(3));
        }
        else if (_unit.UnitId == -1)
        {
            GameManager.Instance.RequestPopulationImprove(10);
        }
    }

    public override void Exit()
    {
        if (_unit.UnitId == -2)
        {
            _unit.StopCoroutine(coroutine);

        }
        //막사면 인구수 감소
        if(_unit.UnitId == -1)
        {
            GameManager.Instance.RequestPopulationImprove(-10);
        }
        CastleManager.Instance.RemoveUnion(_unit.transform, _unit.IsTagAlly());
    }
}