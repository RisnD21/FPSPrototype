using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Patrolling : StateBase
{
    public override string Name => "Patrolling";
    int nextWaypointIndex;
    List<Transform> patrolWaypoints;
    
    //child class must define its own constructor if not using default(parameterless) one
    public Patrolling(AIAgent agent) : base(agent) { } 
    public override void OnEnter()
    {
        base.OnEnter();

        patrolWaypoints = agent.patrolWaypoints;
        if(patrolWaypoints == null) patrolWaypoints.Add(agent.transform);

        routines.Add(agent.StartCoroutine(Patrol()));
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            int currentWaypointIndex = nextWaypointIndex;

            for (int i = currentWaypointIndex; i < patrolWaypoints.Count; i++)
            {
                var nextPosition = patrolWaypoints[i].position + (Vector3)Random.insideUnitCircle * 1.5f;
                if (agent.TryMoveTo(nextPosition))
                {
                    yield return new WaitUntil(() => agent.HasReachDestination());

                    if (agent.isDebugMode) Debug.Log($"[Patrolling] Reach {patrolWaypoints[i].gameObject.name}");
                    Transform[] viewPoints = patrolWaypoints[i].GetComponentsInChildren<Transform>()
                        .Where(t => t != patrolWaypoints[i]).ToArray();

                    foreach (var viewPoint in viewPoints)
                    {
                        if (agent.isDebugMode) Debug.Log("[Patrolling] Checking " + viewPoint.gameObject.name);
                        Vector3 view = viewPoint.position + (Vector3)Random.insideUnitCircle;
                        yield return agent.Observe(view);
                    }
                }

                nextWaypointIndex++;
            }
            nextWaypointIndex = 0;
            yield return null;
        }
    }

    public override void OnUpdate() { }
}