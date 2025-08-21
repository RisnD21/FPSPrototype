using System.Collections;
using UnityEngine;
// attack：若玩家在射程內，則開火；否則進入追擊模式
public class Attacking : StateBase
{
    public override string Name => "Attacking";
    float fireCooldown = 0f;
    bool hasInitialize;
    ActionStates actionController;
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
                // 等待轉向完成
                yield return agent.LookAt(agent.player.position, 0.2f);
            }

            // 一旦轉向完成，就代表可以射擊
            readyToFire = true;
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
        if(agent.player == null) if(agent.isDebugMode) Debug.Log("[Attacking] Target Down");
    }
}