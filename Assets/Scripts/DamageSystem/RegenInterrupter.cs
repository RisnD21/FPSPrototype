using UnityEngine;

public class RegenInterrupter : MonoBehaviour, IHealthListener
{
    [SerializeField] Damageable body;
    [SerializeField] RegenOverTime regenController;

    public void OnHealthChanged(int current, int max, bool hide = false) { }
    public void OnDamaged(int amount, float duration = 0f, bool hide = false)
    {
        if (body != null && regenController != null)
        {
            regenController.InterruptRegen();
        }
    }

    public void OnHealed(int amount, float duration = 0f, bool hide = false) { }
    public void OnDeath() { }
}