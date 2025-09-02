using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StateMonitor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image state;
    [SerializeField] RectTransform alertParent;
    [SerializeField] Image alertBackground;
    [SerializeField] Image alertForeground;

    // ===== Tunables (for easy tweaking) =====
    [Header("Alert Meter")]
    [Range(0, 1)][SerializeField] float maxAlertAlpha = 0.3f;
    [SerializeField] float alertFullThreshold = 90f; // should align w/ setting in AIAgent
    [SerializeField] float alertnessFadeDuration = 1f;

    [Header("Smoothing / Steps")]
    [SerializeField] float stepInterval = 0.1f;

    [Header("State Icon Display")]
    [SerializeField] float stateDisplayDuration = 2f;
    [SerializeField] float stateDisplayInterval = 5f;
    [SerializeField] float stateSwapCooldown = 1.5f;

    [Header("Pop Effect")]
    [SerializeField] float popScale = 1.5f;
    [SerializeField] float popDuration = 0.3f;

    // ===== Internal State =====
    float stateDisplayTimer;
    float swapCooldownTimer;
    float stepTimer;
    bool hideAlertMeter;

    Color initStateIconColor;
    Color alertIconBackgroundColor;
    Color alertIconForegroundColor;
    Vector3 initScale;
    float alertnessPercentage;

    Tween stepTween;
    Tween currentTween;

    void Awake()
    {
        HideState();
    }

    void Start()
    {
        stateDisplayTimer = stateDisplayInterval;

        initStateIconColor = state.color;
        alertIconBackgroundColor = alertBackground.color;
        alertIconForegroundColor = alertForeground.color;
        initScale = state.rectTransform.localScale;

        UpdateMeter(0);
    }

    public void UpdateMeter(float value)
    {
        if (hideAlertMeter && value >= alertFullThreshold) return;

        if (value < alertFullThreshold && stepTimer > 0) return; // 若未達臨界則吃節流計時
        stepTimer = stepInterval;

        hideAlertMeter = false;
        alertForeground.color = alertIconForegroundColor;

        float percentage = Mathf.InverseLerp(0f, alertFullThreshold, value);
        StepOnce(percentage);

        if (percentage == 1f)
        {
            PlayPopEffect(alertParent);
            hideAlertMeter = true;
        }
    }

    void StepOnce(float targetPercentage)
    {
        stepTween?.Kill();

        stepTween = DOTween.To(
                () => alertnessPercentage,
                v => { alertnessPercentage = v; ApplyVisuals(v); },
                targetPercentage,
                stepInterval)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }

    void ApplyVisuals(float percentage)
    {
        float alphaValue = percentage * maxAlertAlpha;
        alertIconBackgroundColor.a = alphaValue;
        alertBackground.color = alertIconBackgroundColor;

        alertForeground.fillAmount = percentage;
    }

    void PlayPopEffect(RectTransform targetObject)
    {
        DOTween.Kill(targetObject);

        Sequence seq = DOTween.Sequence();
        seq.Append(targetObject.DOScale(popScale, popDuration).SetEase(Ease.OutBack))   // scale up
           .Append(targetObject.DOScale(1f, popDuration).SetEase(Ease.InBack))         // scale down
           .Append(alertBackground.DOFade(0f, alertnessFadeDuration))
           .Append(alertForeground.DOFade(0f, alertnessFadeDuration))
           .SetId(targetObject)
           .SetLink(gameObject);
    }

    public void SetStateIcon(string value)
    {
        if (!StateIconLoader.IsReady) return;
        var sprite = StateIconLoader.Set.GetSprite(value);
        
        if (sprite == null)
        {
            Debug.Log("sprite is null");
            HideState();
            return;
        }

        state.sprite = sprite;

        if (swapCooldownTimer > 0) return;
        PlayStateAnim();

        stateDisplayTimer = stateDisplayInterval;
        swapCooldownTimer = stateSwapCooldown;
    }

    void PlayStateAnim()
    {
        currentTween?.Kill();

        state.enabled = true;
        state.color = initStateIconColor;
        state.rectTransform.localScale = Vector3.zero;

        currentTween = state.transform
            .DOScale(initScale, 0.2f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                currentTween = state
                    .DOFade(0f, 0.5f)
                    .SetDelay(stateDisplayDuration)
                    .OnComplete(HideState)
                    .SetLink(gameObject);
            });
    }

    void HideState()
    {
        state.enabled = false;
    }

    void Update()
    {
        if (stateDisplayTimer > 0f) stateDisplayTimer -= Time.deltaTime;
        else
        {
            PlayStateAnim();
            stateDisplayTimer = stateDisplayInterval;
        }

        if (swapCooldownTimer > 0f) swapCooldownTimer -= Time.deltaTime;
        if (stepTimer > 0f) stepTimer -= Time.deltaTime;
    }
}