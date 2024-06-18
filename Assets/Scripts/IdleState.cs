using UnityEngine;

public class IdleState : IState
{
    private readonly Unit _character;
    

    public IdleState(Unit character)
    {
        _character = character;
    }

    public void Enter()
    {
        Debug.Log("Entering Idle State");
        // Idle 상태로 전환할 때 필요한 초기 설정
        // 소환 후 대기상태 종료 후 MoveState 진입
        _character.StartCoroutine(_character.Spawn_Init());
    }

    public void ExecuteUpdate()
    {
        // Idle 상태에서 실행할 로직
        _character.OnValueChanged_SpawnSlider(Time.deltaTime);
    }

    public void ExecuteFixedUpdate()
    {

    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
        // Idle 상태에서 나갈 때 필요한 정리 작업
    }
}

public class MoveState : IState
{
    private readonly Unit _character;

    public MoveState(Unit character)
    {
        _character = character;
    }

    public void Enter()
    {
        Debug.Log("Entering Walking State");
        // Walking 상태로 전환할 때 필요한 초기 설정
    }

    public void ExecuteUpdate()
    {
        // Idle 상태에서 실행할 로직
        return;

    }

    public void ExecuteFixedUpdate()
    {

    }

    public void Exit()
    {
        Debug.Log("Exiting Walking State");
        // Walking 상태에서 나갈 때 필요한 정리 작업
    }
}