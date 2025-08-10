using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class ShieldDamageHandler : MonoBehaviour, IHealthListener, IDamageHandler
{
    Damageable shield;

    void Awake() => shield = GetComponent<Damageable>();

    public int HandleDamage(int amount)
    {
        int before = shield.Current;
        shield.TakeDamage(amount);
        int consumed = before - shield.Current;
        return amount - consumed;
    }

    public void OnHealthChanged(int current, int max){} //effect
    public void OnDamaged(int amount){}   // effect
    public void OnHealed(int amount){}    // positive
    public void OnDeath(){} //effect
}