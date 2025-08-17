using UnityEngine;
using System.Collections.Generic;

public abstract class StateBase : IState
{
    protected AIAgent agent;
    protected List<Coroutine> routines = new();
    protected IState requested;
    protected int requestedPriority;

    public StateBase(AIAgent agent) => this.agent = agent;
    public virtual void OnEnter()
    {
        requested = null;
        if (agent.isDebugMode) Debug.Log($"{agent.gameObject.name} start {GetType().Name}");
    }
    public abstract void OnUpdate();
    public virtual void OnExit()
    {
        foreach (var routine in routines) if (routine != null) agent.StopCoroutine(routine);
        if (agent.isDebugMode) Debug.Log($"{agent.gameObject.name} stop {GetType().Name}");
    }

    protected void RequestTransition(IState state, int priority = -1)
    {
        agent.EnqueueTransition(state, priority);
    }
}