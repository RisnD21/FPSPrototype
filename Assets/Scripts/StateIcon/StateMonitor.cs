using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StateMonitor : MonoBehaviour
{
    [SerializeField] Image state;
    [SerializeField] Image alertBackground;
    [SerializeField] Image alert;
    [SerializeField] TextMeshProUGUI meter;

    float stateDisplayDuration = 2f;
    float stateDisplayInterval = 5f;
    float stateDisplayTimer;
    float stateSwapCooldown = 1.5f;
    float swapCooldownTimer;

    Color initColor;
    Vector3 initScale;

    Tween currentTween;
    void Awake()
    {
        HideState();
    }

    void Start()
    {
        stateDisplayTimer = stateDisplayInterval;
        initColor = state.color;
        initScale = state.rectTransform.localScale;

        UpdateMeter(0);
    }

    public void UpdateMeter(float value)
    {
        int intValue = Mathf.FloorToInt(value);
        meter.text = "Alert " + intValue.ToString();
    }

    public void SetStateIcon(string value)
    {
        if(swapCooldownTimer > 0) return;

        if(!StateIconLoader.IsReady)
        {
            Debug.Log("StateIconLoader is not ready yet");
            return;
        }
        var sprite = StateIconLoader.Set.GetSprite(value);
        if(sprite == null) 
        {
            Debug.Log("sprite is null");
            HideState();
            return;
        }

        state.sprite = sprite;
        PlayStateAnim();

        stateDisplayTimer = stateDisplayInterval;
        swapCooldownTimer = stateSwapCooldown;
    }

    void PlayStateAnim()
    {
        currentTween?.Kill();

        state.enabled = true;
        state.color = initColor;
        state.rectTransform.localScale = Vector3.zero;

        currentTween = state.transform
            .DOScale(initScale, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => 
            {
                currentTween = state
                    .DOFade(0, 0.5f)
                    .SetDelay(stateDisplayDuration)
                    .OnComplete(HideState);
            });
    }

    void HideState()
    {
        state.enabled = false;
    }

    void Update()
    {
        if(stateDisplayTimer > 0) stateDisplayTimer -= Time.deltaTime;
        else 
        {
            PlayStateAnim();
            stateDisplayTimer = stateDisplayInterval;
        }

        if(swapCooldownTimer > 0) swapCooldownTimer -= Time.deltaTime;
    }
}
