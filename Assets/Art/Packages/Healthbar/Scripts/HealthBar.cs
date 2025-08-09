using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IHealthListener
{
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    public void OnHealthChanged(int current, int max)
    {
        if (slider.maxValue != max) slider.maxValue = max;
        slider.value = current;
        if (fill != null) fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void OnDamaged(int value) { } // 不用
    public void OnHealed(int value)  { } // 不用
    public void OnDeath() { }
}