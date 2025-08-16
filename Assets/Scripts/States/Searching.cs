
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
        Initialize();
        
        if(agent.isDebugMode) Debug.Log("[Searching] Start Searching");
        coroutine = agent.StartCoroutine(Search()); 
    }

    void Initialize()
    {
        
        nextState = null;
    }

    IEnumerator Search()
    {
        if(agent.isAlert)
        {
            agent.isAlert = false;

            if(agent.TryMoveTo(agent.lastSeenPlayerPos))
                yield return new WaitUntil(() => agent.HasReachDestination());
        }

        if(agent.player != null)
        {
            yield return agent.StartCoroutine(agent.Observe(agent.player.position, 3f));
            yield return agent.StartCoroutine(agent.Observe(-agent.transform.up, 3f));
        }else yield return null;

        if(agent.isDebugMode) Debug.Log("[Searching] Target lost, return to duty");

        nextState = agent.patrolling;
    }


    public void OnUpdate()
    {
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
        else if(agent.beingHit && agent.player != null)
        {
            agent.lastSeenPlayerPos = agent.player.position;
            nextState = agent.chasing;

            if (agent.isDebugMode) Debug.Log("[Searching] Updating lastSeenPlayerPos");
        }else if(agent.isAlert)
        {
            if (agent.isDebugMode) Debug.Log("[Searching] Checking suspiscious sound");
            nextState = agent.searching;
        }
            
        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }
    public void OnExit()
    {
        agent.StopCoroutine(coroutine);
    }
}