using System.Collections;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Damageable), typeof(RegenOverTime))]
public class ShieldDamageHandler : MonoBehaviour, IHealthListener, IDamageHandler
{
    Damageable shield;
    CircleCollider2D shieldCollider;
    ParticleSystemRenderer shieldRenderer;
    MaterialPropertyBlock mpb;
    RegenOverTime regenerator;
    
    [Header("Property")]
    [SerializeField] float regenSpeed = 3;
    [SerializeField] float rechargeCooldown = 2;
    Coroutine ShieldCooldownRoutine;

    int alphaID;
    float idleAlpha = 0.1f;
    float hitAlpha = 1f;
    float fadeDuration = 3f; // 從 hitAlpha 淡回 idleAlpha 的時間
    Tween fadeTween;

    void Awake()
    {
        shield = GetComponent<Damageable>();
        shieldCollider = GetComponent<CircleCollider2D>();
        shieldRenderer = GetComponent<ParticleSystemRenderer>();
        mpb = new MaterialPropertyBlock();
        alphaID = Shader.PropertyToID("_Alpha");

        regenerator = GetComponent<RegenOverTime>();
    }

    void Start()
    {
        SetAlpha(idleAlpha);
    }

    public void SetProperty(float regenSpeed, float rechargeCooldown)
    {
        this.regenSpeed = regenSpeed;
        this.rechargeCooldown = rechargeCooldown;
        Debug.Log($"Setting shield, regenerate speed = {this.regenSpeed}, recharge cd = {this.rechargeCooldown}");
        StartRegenerate();
    }
    
    public int HandleDamage(int amount)
    {
        if(!gameObject.activeSelf) return amount;

        int before = shield.Current;
        shield.TakeDamage(amount);
        int consumed = before - shield.Current;
        return amount - consumed;
    }

    void SetAlpha(float a)
    {
        shieldRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(alphaID, Mathf.Clamp01(a));
        shieldRenderer.SetPropertyBlock(mpb);
    }

    float GetAlpha()
    {
        shieldRenderer.GetPropertyBlock(mpb);
        return mpb.GetFloat(alphaID);
    }

    public void OnHealthChanged(int current, int max, bool hide = false){}
    public void OnDamaged(int amount, float duration = 0f, bool hide = false)
    {
        FlashOnHit();
        StopRegenerate();
    }
    public void OnHealed(int amount, float duration = 0f, bool hide = false){}
    public void OnDeath()
    {
        fadeTween?.Kill();
        SetAlpha(0f);

        StopRegenerate(7f);
        shieldCollider.enabled = false;
    }

    void FlashOnHit()
    {
        fadeTween?.Kill();
        SetAlpha(hitAlpha);

        fadeTween = DOTween.To(
            () => GetAlpha(),
            a => SetAlpha(a),
            idleAlpha,
            fadeDuration
        ).SetEase(Ease.OutQuad);
    }

    void StopRegenerate(float cooldown = -1)
    {
        if(ShieldCooldownRoutine != null) StopCoroutine(ShieldCooldownRoutine);
        regenerator.InterruptRegen();

        if(cooldown == -1) cooldown = rechargeCooldown;
        ShieldCooldownRoutine = StartCoroutine(RechargeCooldown(cooldown));
    }

    IEnumerator RechargeCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        StartRegenerate();
    }

    void StartRegenerate()
    {
        shield.Revive();
        shieldCollider.enabled = true;

        float regenAmount = shield.Max - shield.Current;
        float duration = regenAmount / regenSpeed;
        Debug.Log($"Start Regenerate Shield, regenAmount = {regenAmount}, duration = {duration}");
        HealingEffect regenEffect = new("shieldRegen", regenAmount, duration, EffectMode.reset, true);
        regenerator.AddEffect(regenEffect);

        fadeTween?.Kill();
        fadeTween = DOTween.To(
            () => GetAlpha(),
            a => SetAlpha(a),
            idleAlpha,
            duration
        ).SetEase(Ease.OutQuad);
    }
    
    void OnDestroy()
    {
        fadeTween?.Kill();
    }
}