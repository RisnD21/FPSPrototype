using UnityEngine;

public class Chasing : StateBase
{   
    public override string Name => "Chasing";
    public Chasing(AIAgent agent) : base(agent) {}

    public override void OnEnter()
    {
        base.OnEnter();
        Vector3 target = agent.blackboard.lastSeenEnemy.Value + (Vector3) Random.insideUnitCircle * 1.5f;

        if(!agent.TryMoveTo(target, agent.chaseSpeed)) 
        {
            RequestTransition(agent.patrolling);
        }

        agent.blackboard.lastHeardPos = null;
        agent.blackboard.lastHeardPosTimestamp = 0;
        agent.CallReinforcement(agent.blackboard.lastSeenEnemy.Value);
    }

    public override void OnUpdate()
    {
        if(agent.HasReachDestination()) RequestTransition(agent.searching);
    }
    public override void OnExit()
    {
        base.OnExit();
        agent.Halt();
    }
}