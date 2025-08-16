using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Chatting : IState
{
    AIAgent agent;
    Coroutine routine;
    
    IState nextState;
    bool hasInitialize;

    public Chatting(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log($"[Chatting] {agent.gameObject.name} start chatting");

        Initialize();
        agent.BackToNormalState();

        routine = agent.StartCoroutine(ChattingWith(agent.allyToChat));
    }

    void Initialize()
    {
        if (!hasInitialize)
        {
            hasInitialize = true;
        }
        
        nextState = null;
    }

    IEnumerator ChattingWith(AIAgent agent)
    {
        yield return agent.StartCoroutine(agent.LookAt(agent.transform.position));
        yield return new WaitForSeconds(10f);
        if(agent.TryFindAllyToChat()) nextState = agent.chatting;
        else
        {
            agent.StopChatting();
            nextState = agent.patrolling;
        }
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
        agent.StopCoroutine(routine);
    }
}