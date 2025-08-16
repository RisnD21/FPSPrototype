using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Patrolling : IState
{
    AIAgent agent;
    Coroutine routine;
    
    int nextWaypointIndex;
    List<Transform> patrolWaypoints;

    IState nextState;
    bool hasInitialize;

    float chatCooldown = 5f;
    float chatCountdown;

    public Patrolling(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log("[Patrolling] Start patrolling");

        Initialize();

        routine = agent.StartCoroutine(Patrol());
        agent.BackToNormalState();
    }

    void Initialize()
    {
        if (!hasInitialize)
        {
            patrolWaypoints = agent.patrolWaypoints;

            hasInitialize = true;
        }
        
        nextState = null;
    }

    IEnumerator Patrol()
    {
        int currentWaypointIndex = nextWaypointIndex;

        for (int i = currentWaypointIndex; i < patrolWaypoints.Count; i++)
        {
            var nextPosition = patrolWaypoints[i].position;
            if(agent.TryMoveTo(nextPosition))
            {
                yield return new WaitUntil(() => agent.HasReachDestination());

                if (agent.isDebugMode) Debug.Log($"[Patrolling] Reach {patrolWaypoints[i].gameObject.name}");
                Transform[] viewPoints = patrolWaypoints[i].GetComponentsInChildren<Transform>()
                    .Where(t => t!=patrolWaypoints[i]).ToArray();

                
                foreach (var viewPoint in viewPoints)
                {
                    if (agent.isDebugMode) Debug.Log("[Patrolling] Checking " + viewPoint.gameObject.name);
                    yield return agent.StartCoroutine(agent.Observe(viewPoint.position));
                }
            }

            nextWaypointIndex++;
        }

        nextWaypointIndex = 0;
        routine = agent.StartCoroutine(Patrol());
    }

    public void OnUpdate()
    {
        if(agent.IsPlayerInSight()) nextState = agent.attacking;
        else if(agent.beingHit && agent.player != null)
        {
            agent.lastSeenPlayerPos = agent.player.position;
            nextState = agent.chasing;
        }else if(agent.isAlert) nextState = agent.searching;

        if (agent.needChat) nextState = agent.chatting;
        else if (WantToChat() && agent.TryFindAllyToChat()) nextState = agent.chatting;

        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }

    bool WantToChat()
    {
        if(chatCountdown > 0)
        {
            chatCountdown -= Time.deltaTime;
            return false;
        }else
        {
            chatCountdown = chatCooldown;
            return true;
        }        
    }

    public void OnExit() 
    {
        agent.StopCoroutine(routine);
    }
}