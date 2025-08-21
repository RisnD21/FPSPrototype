using System.Collections;
using UnityEngine;


public class Chatting : StateBase
{
    public override string Name => "Chatting";
    public Chatting(AIAgent agent) :base(agent){}
    float duration;

    public override void OnEnter()
    {
        base.OnEnter();
        routines.Add(agent.StartCoroutine(ChattingWith(agent.blackboard.allyToChat)));
        duration = agent.blackboard.chatDuration;
    }

    IEnumerator ChattingWith(AIAgent ally)
    {
        Debug.Log($"[Chatting] {agent.gameObject.name} is chatting with {ally.gameObject.name}");
        
        yield return agent.Observe(ally.transform.position, duration);
        
        yield return new WaitForSeconds(duration);

        Debug.Log($"[Chatting] {agent.gameObject.name} has chatted for {duration} sec");

        RequestTransition(agent.patrolling);
    }

    public override void OnUpdate(){}

    public override void OnExit() 
    {
        base.OnExit();
        agent.blackboard.allyToChat = null;
        agent.blackboard.chatDuration = 0;
        agent.blackboard.chatDesire = 0;
    }
}