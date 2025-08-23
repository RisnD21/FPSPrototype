using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class HealingEffect
{
    public string id;
    public float amount;
    public float duration;
    public float remainingAmount;
    public float regenSpeed;
    public EffectMode mode;

    public bool canBeDisrupted;
    public HealingEffect(string id, float amount, float duration, EffectMode mode, bool canBeDisrupted)
    {
        this.id = id;
        this.amount = amount;
        remainingAmount = amount + 0.1f;
        regenSpeed = Mathf.Round(amount * 100f / duration) /100f;
        this.duration = Mathf.Max(0,duration);
        this.mode = mode;
        this.canBeDisrupted = canBeDisrupted;
    }

    public override bool Equals(object obj)
    {            
        if (obj != null && obj is HealingEffect other)
        {
            return other.id == id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        if (id == null) return 0;
        return id.GetHashCode();
    }
}

public enum EffectMode
{
    reset,
    accumulate,
    parallel
}

[RequireComponent(typeof(Damageable))]
public class RegenOverTime : MonoBehaviour
{
    Damageable target;

    List<HealingEffect> effects;    //allow duplicaate
    Dictionary<string, HealingEffect> index; //paring id & effects, doesn't sync with effects
    float accumulateHealingAmount;
    void Awake()
    {
        effects = new();
        index = new();
        target = GetComponent<Damageable>();

        StartCoroutine(ApplyAccumulatedHealing());
    }

    public void AddEffect(HealingEffect effect)
    {
        HealingEffect prevEffect = effects.Find(e => e.Equals(effect));

        if(prevEffect == null)
        {
            effects.Add(effect);
            return;
        }

        switch (effect.mode)
        {
            case EffectMode.reset:
            effects.Remove(prevEffect);
            effects.Add(effect);
            break;

            case EffectMode.parallel:
            effects.Add(effect);
            break;

            case EffectMode.accumulate:
            prevEffect.remainingAmount += effect.remainingAmount;
            break;

            default:
            break;
        }
    }

    public void InterruptRegen()
    {
        foreach(var effect in effects) if(effect.canBeDisrupted) effect.remainingAmount = 0;
    }

    void Update()
    {
        ApplyEffects();
    }

    void ApplyEffects()
    {
        int effectCount = effects.Count;
        if(effectCount == 0) return;

        float dt = Time.deltaTime;

        for(int i = effectCount - 1; i >= 0; i--)
        {
            var effect = effects[i];

            float regenAmount = effect.regenSpeed * dt;

            float actualRegenAmount = Mathf.Min(effect.remainingAmount, regenAmount);
            accumulateHealingAmount += actualRegenAmount;
            effect.remainingAmount -= actualRegenAmount;   

            if(effect.remainingAmount <= 0) effects.RemoveAt(i);
        }
    }

    IEnumerator ApplyAccumulatedHealing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if(accumulateHealingAmount > 1)
            {
                int healSurge = Mathf.FloorToInt(accumulateHealingAmount);
                
                target.Heal(healSurge, 1, false);
                accumulateHealingAmount -= healSurge;
            }
        }
    }
}