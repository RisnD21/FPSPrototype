using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IHealthListener
{
    [SerializeField] Slider frontSlider;
    [SerializeField] Slider backSlider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    Tween currentTween;
    float minDuration = 0.5f;

    public void OnHealthChanged(int current, int max, bool hide = false)
    {
        if (frontSlider.maxValue != max)
        {
            frontSlider.maxValue = max;
            frontSlider.value = current;
        }
        if (backSlider.maxValue != max)
        {
            backSlider.maxValue = max;
            backSlider.value = current;
        }

        //only front slider has gradient color
        if (fill != null) fill.color = gradient.Evaluate(frontSlider.normalizedValue);
    }

    public void OnDamaged(int value, float duration = 0f, bool hide = false)
    {
        float targetValue = Mathf.Max(frontSlider.value - value, 0);
        SetSlider(frontSlider, targetValue);
        SetSlider(backSlider, targetValue, duration);
    }
    public void OnHealed(int value, float duration = 0f, bool hide = false)
    {
        if(value <= 0) return;
        float targetValue = Mathf.Min(backSlider.value + value, backSlider.maxValue);
        SetSlider(backSlider, targetValue);
        SetSlider(frontSlider, targetValue, duration);
    }
    public void OnDeath() 
    {
        SetSlider(frontSlider, 0);
        SetSlider(backSlider, 0);
    }

    void SetSlider(Slider targetSlider, float targetValue, float duration = 0f)
    {
        if(duration == 0)
        {
            targetSlider.value = targetValue;
            return;
        }

        duration = Mathf.Max(minDuration, duration);

        currentTween?.Kill();
        currentTween = DOTween.To(
            () => targetSlider.value,
            v => UpdateSlider(targetSlider,v),
            targetValue,
            duration);
    }

    void UpdateSlider(Slider slider, float value)
    {
        slider.value = value;
        if(slider != frontSlider) return;
        if (fill != null) fill.color = gradient.Evaluate(frontSlider.normalizedValue);
    }
}