using System.Collections;
using UnityEngine;

public class Observing : IState
{
    AIAgent agent;
    IState nextState;


    Coroutine coroutine;
    public Observing(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log($"[Observing] {agent.gameObject.name} starts attacking");
        Initialize();
        agent.Halt();
    }

    void Initialize()
    {   
        nextState = null;
    }

    public void OnUpdate()
    {   
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
        else if(agent.beingHit && agent.player != null)
        {
            agent.lastSeenPlayerPos = agent.player.position;
            nextState = agent.chasing;
        }else if(agent.isAlert) nextState = agent.searching;

        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }

    public void OnExit()
    {
        agent.StopCoroutine(coroutine);
    }
}