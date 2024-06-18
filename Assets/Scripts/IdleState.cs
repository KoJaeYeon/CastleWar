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
        // Idle ���·� ��ȯ�� �� �ʿ��� �ʱ� ����
        // ��ȯ �� ������ ���� �� MoveState ����
        _character.StartCoroutine(_character.Spawn_Init());
    }

    public void ExecuteUpdate()
    {
        // Idle ���¿��� ������ ����
        _character.OnValueChanged_SpawnSlider(Time.deltaTime);
    }

    public void ExecuteFixedUpdate()
    {

    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
        // Idle ���¿��� ���� �� �ʿ��� ���� �۾�
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
        // Walking ���·� ��ȯ�� �� �ʿ��� �ʱ� ����
    }

    public void ExecuteUpdate()
    {
        // Idle ���¿��� ������ ����
        return;

    }

    public void ExecuteFixedUpdate()
    {

    }

    public void Exit()
    {
        Debug.Log("Exiting Walking State");
        // Walking ���¿��� ���� �� �ʿ��� ���� �۾�
    }
}