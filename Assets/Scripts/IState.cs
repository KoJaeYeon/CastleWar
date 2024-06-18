public interface IState
{
    void Enter();
    void ExecuteUpdate();
    void ExecuteFixedUpdate();
    void Exit();
}