
using UnityEngine;

public class Chasing : IState
{
    AIAgent agent;
    IState nextState;
    
    public Chasing(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log("[Chasing] Start Chasing");

        Initialize();
        if(!agent.TryMoveTo(agent.lastSeenPlayerPos, agent.chaseSpeed)) 
        {
            nextState = agent.patrolling;
        }

        agent.CallReinforcement();
    }
    
    void Initialize()
    {
        nextState = null;
    }

    public void OnUpdate()
    {
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
        else if(agent.HasReachDestination()) nextState = agent.searching;
        
        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }
    public void OnExit()
    {
        agent.beingHit = false;
        agent.Halt();
    }
}