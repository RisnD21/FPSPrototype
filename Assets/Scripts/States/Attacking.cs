using System.Collections;
using UnityEngine;
// attack：若玩家在射程內，則開火；否則進入追擊模式
public class Attacking : IState
{
    AIAgent agent;
    IState nextState;
    float triggerFrequency = 1f;
    
    float timer = 0f;
    bool hasInitialize;
    bool readyToFire;
    bool isAiming;
    ActionStates actionController;
    Coroutine coroutine;
    public Attacking(AIAgent agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        if(agent.isDebugMode) Debug.Log("Start attacking");
        Initialize();
        agent.Halt();
        coroutine = agent.StartCoroutine(Aim());
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

        timer = 0;
        readyToFire = false;
        isAiming = false;
        nextState = null;
    }

    IEnumerator Aim()
    {
        isAiming = true;

        while (isAiming && agent.player!=null)
        {
            Vector2 aimingDir = agent.player.position - agent.transform.position;
            if (Vector2.Angle(agent.transform.up, aimingDir) > 1)
            {
                yield return agent.StartCoroutine(agent.LookAt(agent.player.position, 0.3f));
            }

            readyToFire = true;
            yield return null;
        }
    }

    public void OnUpdate()
    {   
        UpdateCooldown();

        if(!agent.IsPlayerInSight()) nextState = agent.chasing;
        if(agent.player == null) nextState = agent.patrolling;

        if(nextState == null) return;
        agent.TransitionTo(nextState);
    }

    void UpdateCooldown()
    {
        if (timer <= 0 && readyToFire)
        {
            actionController.Fire();
            timer = triggerFrequency;
            readyToFire = false;
        }
        actionController.StopFire();
        timer -= Time.deltaTime;
    }

    public void OnExit()
    {
        if(agent.player != null)
            agent.lastSeenPlayerPos = agent.player.position;
        agent.StopCoroutine(coroutine);
    }
}