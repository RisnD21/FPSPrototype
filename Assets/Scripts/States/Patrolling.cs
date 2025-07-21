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

    public Patrolling(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        patrolWaypoints = agent.patrolWaypoints;
        
        routine = agent.StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        int currentWaypointIndex = nextWaypointIndex;

        for (int i = currentWaypointIndex; i < patrolWaypoints.Count; i++)
        {
            var nextPosition = patrolWaypoints[i].position;
            agent.MoveTo(nextPosition);

            //wait until reach nextPosition
            yield return new WaitUntil(() => agent.HasReachPosition(nextPosition));

            Transform[] viewPoints = patrolWaypoints[i].GetComponentsInChildren<Transform>()
                .Where(t => t!=patrolWaypoints[i]).ToArray();

            foreach (var viewPoint in viewPoints)
            {
                yield return agent.StartCoroutine(agent.Observe(viewPoint.position));
            }

            nextWaypointIndex++;
        }

        nextWaypointIndex = 0;
        routine = agent.StartCoroutine(Patrol());
    }

    public void OnUpdate()
    {

    }
    public void OnExit() 
    {
        agent.StopCoroutine(routine);
    }
}