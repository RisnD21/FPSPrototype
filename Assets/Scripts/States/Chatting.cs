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

        routine = agent.StartCoroutine(ChattingWith(agent.allyToChat));
    }

    void Initialize()
    {
        if (!hasInitialize)
        {
            hasInitialize = true;
        }

        nextState = null;
        
        agent.BackToNormalState();
        agent.isChatting = true;
        agent.needChat = false;
    }

    IEnumerator ChattingWith(AIAgent ally)
    {
        Debug.Log($"[Chatting] {agent.gameObject.name} is chatting with {ally.gameObject.name}");
        
        yield return agent.StartCoroutine(agent.LookAt(ally.transform.position));
        yield return new WaitForSeconds(agent.chatDuration);

        Debug.Log($"[Chatting] {agent.gameObject.name} has chatted for {agent.chatDuration} sec");
        nextState = agent.patrolling;
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
        agent.StopChatting();
        agent.StopCoroutine(routine);
    }
}