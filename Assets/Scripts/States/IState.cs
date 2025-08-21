public interface IState
{
    string Name {get;}
    void OnEnter();
    void OnUpdate();
    void OnExit();
}