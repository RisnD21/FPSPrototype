
using UnityEngine.Timeline;

public class Attacking : IState
{
    AIAgent agent;
    float patrolCooldown = 30f;
    

    public Attacking(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {

    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {

    }

    // patrol：定時前往特定地點，地點請於 inspector 中設定
}