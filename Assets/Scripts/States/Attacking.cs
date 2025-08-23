using System.Collections;
using UnityEngine;
// attack：若玩家在射程內，則開火；否則進入追擊模式
public class Attacking : StateBase
{
    public override string Name => "Attacking";
    float fireCooldown = 0f;
    bool hasInitialize;
    ActionStates actionController;
    FireLaneSensor2D sensor;
    StrafePlanner2D strafePlanner;
    public Attacking(AIAgent agent) : base(agent) { }
    bool readyToFire;

    public override void OnEnter()
    {
        base.OnEnter();

        Initialize();
        agent.Halt();
        agent.CallReinforcement(agent.player.position);
        routines.Add(agent.StartCoroutine(Aim()));
    }

    void Initialize()
    {
        if (!hasInitialize)
        {
            actionController = agent.GetComponentInChildren<ActionStates>();
            sensor = agent.GetComponentInChildren<FireLaneSensor2D>();
            strafePlanner = agent.GetComponentInChildren<StrafePlanner2D>();

            if (actionController == null)
            {
                Debug.LogError("[Attacking] ActionStates missing");
            }
            hasInitialize = true;
        }
    }

    IEnumerator Aim()
    {
        while (agent.player != null)
        {
            Vector2 aimingDir = agent.player.position - agent.transform.position;

            if (Vector2.Angle(agent.transform.up, aimingDir) > 1f)
            {
                yield return agent.LookAt(agent.player.position, 0.2f);
            }

            if(sensor.HasFriendlyInFireLane(agent.muzzle.position, agent.muzzle.right))
            {
                Vector2? newPos = strafePlanner.GetStrafePos(agent.transform.position, agent.muzzle.position, agent.muzzle.right);

                if (newPos.HasValue) 
                {
                    agent.blackboard.isRePositioning = true;

                    Debug.Log($"{agent.gameObject.name} is repositioning");

                    Vector2 offset = agent.muzzle.position - agent.transform.position;
                    agent.TryMoveTo(newPos.Value + offset);

                    yield return new WaitUntil(agent.HasReachDestination);
                    
                    Debug.Log($"{agent.gameObject.name} is now at {agent.muzzle.position}");

                    agent.blackboard.isRePositioning = false;
                }
                else yield return new WaitForSeconds(0.5f);

                continue;

            } else
            {
                agent.Halt();
                readyToFire = true;
            }
        
            yield return null;
        }
    }

    public override void OnUpdate()
    {
        // 沒目標就切狀態
        if (agent.player == null)
        {
            RequestTransition(agent.patrolling);
            return;
        }

        // 冷卻倒數
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
            return;
        }

        if (readyToFire)
        {
            actionController.Fire();            // 開火當幀
            actionController.StopFire();        // 立刻停火（避免持續射擊）
            fireCooldown = actionController.currentWeapon.fireCooldown;
            readyToFire = false;                // 重置，等下一次 Aim 完成
        }
    }
    public override void OnExit()
    {
        base.OnExit();
        agent.blackboard.isRePositioning = false;
        if(agent.player == null) if(agent.isDebugMode) Debug.Log("[Attacking] Target Down");
    }
}