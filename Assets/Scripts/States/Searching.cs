
using System.Collections;
using UnityEngine;

//往玩家位置看去，若玩家在視線中，進入chase；否則紀錄該方向並微幅隨機張望，10秒後回到待命地點
public class Searching : StateBase
{
    public override string Name => "Searching";
    public Searching(AIAgent agent) : base(agent) {}
    Vector3 currentClue;
    float minDifferenceToUpdateClue = 3f;
    float clueUpdateCooldown = 2f;
    float clueUpdateCountdown;

    public override void OnEnter()
    {
        base.OnEnter();

        if(agent.blackboard.lastHeardPosTimestamp > agent.blackboard.lastSeenEnemyTimestamp)
        {
            currentClue = agent.blackboard.lastHeardPos.Value + (Vector3) Random.insideUnitCircle * 1.5f;
            
            if(agent.isDebugMode) Debug.Log("[Searching] Checking suspiscious sound");
        }else
        {
            currentClue = agent.blackboard.lastSeenEnemy.Value + (Vector3) Random.insideUnitCircle * 1.5f;
            if(agent.isDebugMode) Debug.Log("[Searching] Enemy lost sight, start searching");
        }

        routines.Add(agent.StartCoroutine(Search())); 
    }

    IEnumerator Search()
    {
        if(agent.TryMoveTo(currentClue))
            yield return new WaitUntil(() => agent.HasReachDestination());

        if(agent.player != null)
        {
            yield return agent.Observe(agent.player.position, 3f);
            yield return agent.Observe(-agent.transform.up, 3f);
        }

        if(agent.isDebugMode) Debug.Log("[Searching] Target lost for sure, return to duty");
        RequestTransition(agent.patrolling);
    }

    bool TryUpdateClue(Vector3? newClue)
    {
        if(!newClue.HasValue) return false;
        Vector3 newPosition = newClue.Value;
        if(Vector3.Distance(newPosition, currentClue) > minDifferenceToUpdateClue)
        {
            currentClue = newPosition;
            
            return true;
        }
        return false;
    }

    public override void OnUpdate()
    {
        if(clueUpdateCountdown > 0) clueUpdateCountdown -= Time.deltaTime;
        else
        {
            if(TryUpdateClue(agent.blackboard.lastHeardPos)) 
            {
                agent.Halt();
                Debug.Log("[Searching] Clue updated, searching new clue");
                foreach (var routine in routines) agent.StopCoroutine(routine);
                routines.Add(agent.StartCoroutine(Search())); 
            }

            clueUpdateCountdown = clueUpdateCooldown;
        }
    }
    public override void OnExit()
    {
        base.OnExit();

        agent.blackboard.lastHeardPos = null;
        agent.blackboard.lastHeardPosTimestamp = 0;
    }
}