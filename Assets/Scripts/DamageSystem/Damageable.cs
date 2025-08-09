using UnityEngine;
using System.Linq;

public class Damageable : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] MonoBehaviour[] listenerBehaviors; //drag IHealthListener
    IHealthListener[] listeners;
    public int Current {get; private set;}
    public int Max => maxHealth;
    bool isDead;

    void Awake()
    {
        listeners = listenerBehaviors?.OfType<IHealthListener>().ToArray();
        Current = maxHealth;
        NotifyChanged();
    }

    public void Heal(int value)
    {
        if(isDead || value <= 0) return;

        int before = Current;
        Current = Mathf.Clamp(before + value, 0, maxHealth);
        
        int healed = Current - before;
        if(healed <= 0) return;
        foreach (var l in listeners) l.OnHealed(healed);
        NotifyChanged();
    }

    public void TakeDamage(int value)
    {
        if(isDead || value <= 0) return;
        int before = Current;
        Current = Mathf.Clamp(before - value, 0, maxHealth);
        
        int taken = before - Current;
        if(taken <= 0) return;

        foreach(var l in listeners) l.OnDamaged(taken);
        NotifyChanged();

        if(Current == 0 && !isDead)
        {
            isDead = true;
            foreach(var l in listeners) l.OnDeath();
        }
    }

    void NotifyChanged()
    {
        foreach (var l in listeners) l.OnHealthChanged(Current, maxHealth);
    }
}