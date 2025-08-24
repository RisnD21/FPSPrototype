using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

//Sense impact or heard things
public class Observing : StateBase
{
    public override string Name => "Observing";
    float minDiffToUpdateObesrvePoint = 3f;
    float observePointUpdateInterval = 2f;
    float observePointUpdateTimer;
    Vector3 currentObservePoint;
    public Observing(AIAgent agent) : base(agent){}

    public override void OnEnter()
    {
        base.OnEnter();
        agent.Halt();

        if(agent.blackboard.lastHeardPosTimestamp > agent.blackboard.lastImpactPosTimestamp)
        {
            currentObservePoint = agent.blackboard.lastHeardPos.Value;
            routines.Add(agent.StartCoroutine(ObserveAtPos()));
        }else
        {
            currentObservePoint = agent.blackboard.lastImpactPos.Value;
            routines.Add(agent.StartCoroutine(ObserveAtSrc()));
        }
    }

    IEnumerator ObserveAtPos()
    {
        yield return agent.LookAt(currentObservePoint);
        agent.CallReinforcement(currentObservePoint);

        if(agent.player != null) RequestTransition(agent.searching);
    }

    IEnumerator ObserveAtSrc()
    {
        yield return agent.LookAt(currentObservePoint);
        agent.CallReinforcement(agent.player.position);
        
        if(!agent.TargetInSight(currentObservePoint))
        {
            agent.blackboard.lastHeardPos = currentObservePoint;
            agent.blackboard.lastHeardPosTimestamp = Time.time;
            RequestTransition(agent.searching);
            yield return null;
        }

        agent.blackboard.lastHeardPos = agent.player.position;
        agent.blackboard.lastHeardPosTimestamp = Time.time;
        yield return agent.Observe(agent.player.position, 5);
        
        agent.blackboard.lastImpactPos = null;
        RequestTransition(agent.searching);
    }

    public override void OnUpdate()
    {
        if(observePointUpdateTimer > 0) observePointUpdateTimer -= Time.deltaTime;
        else
        {
            if(agent.blackboard.lastHeardPosTimestamp > agent.blackboard.lastImpactPosTimestamp)
            { //focus on heard pos
                if(Vector3.Distance(agent.blackboard.lastHeardPos.Value, currentObservePoint) > minDiffToUpdateObesrvePoint)
                {
                    RequestTransition(agent.searching);
                }
            }else
            { //focus on impact pos 
                if(Vector3.Distance(agent.blackboard.lastImpactPos.Value, currentObservePoint) > minDiffToUpdateObesrvePoint)
                {
                    agent.blackboard.lastHeardPos = agent.player.position;
                    RequestTransition(agent.searching);
                }
            }
            observePointUpdateTimer = observePointUpdateInterval;
        }
    }
}