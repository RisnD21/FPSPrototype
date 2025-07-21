
using System.Collections;
using System.Collections.Generic;
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
            yield return new WaitUntil(
                () => Vector3.Distance(agent.transform.position,  nextPosition) < 1);

            foreach (var checkPoint in patrolWaypoints[i].GetComponentsInChildren<Transform>())
            {
                yield return agent.StartCoroutine(agent.Observe(checkPoint.position));
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