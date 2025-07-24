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

    public Patrolling(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log("Start patrolling");

        Initialize();

        routine = agent.StartCoroutine(Patrol());
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
                //wait until reach nextPosition
                yield return new WaitUntil(() => agent.HasReachDestination());

                Transform[] viewPoints = patrolWaypoints[i].GetComponentsInChildren<Transform>()
                    .Where(t => t!=patrolWaypoints[i]).ToArray();

                foreach (var viewPoint in viewPoints)
                {
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
        }

        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }
    public void OnExit() 
    {
        agent.StopCoroutine(routine);
    }
}