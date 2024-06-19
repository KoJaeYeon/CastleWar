using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitState : IState
{
    protected Unit _character;

    public virtual void Enter()
    {
    }

    public virtual void ExecuteFixedUpdate()
    {
    }

    public virtual void ExecuteUpdate()
    {
    }

    public virtual void Exit()
    {
    }
}

public class UnitIdleState : UnitState
{  

    public UnitIdleState(Unit character)
    {
        _character = character;
    }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        // Idle 상태로 전환할 때 필요한 초기 설정
        // 소환 후 대기상태 종료 후 MoveState 진입
        _character.StartCoroutine(_character.Spawn_Init());
    }

    public override void ExecuteUpdate()
    {
        // Idle 상태에서 실행할 로직
        _character.OnValueChanged_SpawnSlider(Time.deltaTime);
    }

    public override void ExecuteFixedUpdate()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
        // Idle 상태에서 나갈 때 필요한 정리 작업
    }
}

public class UnitMoveState : UnitState
{
    public UnitMoveState(Unit character)
    {
        _character = character;
    }

    public override void Enter()
    {
        Debug.Log("Entering Walking State");
        // Walking 상태로 전환할 때 필요한 초기 설정
    }

    public override void ExecuteUpdate()
    {
        // Idle 상태에서 실행할 로직
        return;

    }

    public override void ExecuteFixedUpdate()
    {
        _character.SearchEnemy();
        if(_character.TargetChanged)
        {
            //Astar
        }
        Move();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Walking State");
        // Walking 상태에서 나갈 때 필요한 정리 작업
    }

    /*
     * 적을 탐색하는 기능
     * 적을 향해 움직이는 기능
     * 앞으로 움직이는 기능
     * 적을 발견하고 사이에 장애물이 있으면 돌아가는 기능 -> Astar
     */

    public void Move()
    {
        
    }
    
}