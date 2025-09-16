using UnityEngine;
using System.Linq;
using System;

public class Damageable : MonoBehaviour
{
    public int maxHealth = 10;
    [SerializeField] MonoBehaviour[] listenerBehaviors; //drag IHealthListener
    IHealthListener[] listeners;
    public int Current {get; private set;}
    public int Max => maxHealth;
    bool isDead;
    public event Action<float> RecordLoss;
    public event Action<float> RecordRecover;
    public event Action RecordDeath;

    void Awake()
    {
        listeners = listenerBehaviors?.OfType<IHealthListener>().ToArray();
        Current = maxHealth;
        NotifyChanged();
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        Current = Mathf.Min(Current, maxHealth);
        NotifyChanged(true);
    }

    public void Revive()
    {
        isDead = false;
    }

    public void Heal(int value, float duration = 0f, bool hide = false)
    {
        if(isDead || value <= 0) return;

        int before = Current;
        Current = Mathf.Clamp(before + value, 0, maxHealth);
        
        int healed = Current - before;
        if(healed <= 0) return;

        RecordRecover?.Invoke(healed);

        foreach (var l in listeners) l.OnHealed(healed, duration, hide);
        NotifyChanged();
    }

    public void TakeDamage(int value, bool hide = false)
    {
        if(isDead || value <= 0) return;
        int before = Current;
        Current = Mathf.Clamp(before - value, 0, maxHealth);
        
        int taken = before - Current;
        if(taken <= 0) return;

        RecordLoss?.Invoke(taken);

        foreach(var l in listeners) l.OnDamaged(taken, 1f, hide);
        NotifyChanged(hide);

        if(Current == 0 && !isDead)
        {
            isDead = true;
            RecordDeath?.Invoke();
            foreach (var l in listeners) l.OnDeath();
        }
    }

    void NotifyChanged(bool hide = false)
    {
        foreach (var l in listeners) l.OnHealthChanged(Current, maxHealth, hide);
    }
}