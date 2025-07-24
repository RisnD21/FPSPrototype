
using System.Collections;
using UnityEngine;

//往玩家位置看去，若玩家在視線中，進入chase；否則紀錄該方向並微幅隨機張望，10秒後回到待命地點
public class Searching : IState
{
    AIAgent agent;
    IState nextState;
    Coroutine coroutine;
    public Searching(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log("Start Searching");
        coroutine = agent.StartCoroutine(Search());
    }

    IEnumerator Search()
    {
        if(agent.player != null)
        {
            yield return agent.StartCoroutine(agent.Observe(agent.player.position, 3f));
            yield return agent.StartCoroutine(agent.Observe(-agent.transform.up, 3f));
        }else yield return null;
        
        nextState = agent.patrolling;
    }


    public void OnUpdate()
    {
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
    
        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }
    public void OnExit()
    {
        agent.StopCoroutine(coroutine);
    }
}