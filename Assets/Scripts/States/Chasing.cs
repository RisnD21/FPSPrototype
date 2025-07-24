
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
        if(agent.isDebugMode) Debug.Log("Start Chasing");

        Initialize();
        if(!agent.TryMoveTo(agent.lastSeenPlayerPos, agent.chaseSpeed)) 
        {
            nextState = agent.patrolling;
        }      
    }
    
    void Initialize()
    {
        nextState = null;
    }

    public void OnUpdate()
    {
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
        if(agent.HasReachDestination()) nextState = agent.searching;
        
        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }
    public void OnExit()
    {
        agent.beingHit = false;
        agent.Halt();
    }
}