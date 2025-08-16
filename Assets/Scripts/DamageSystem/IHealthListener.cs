//Those who cares health value should implement this
public interface IHealthListener
{
    void OnHealthChanged(int current, int max, bool hide = false);
    void OnDamaged(int amount, float duration = 0f, bool hide = false);   // positive
    void OnHealed(int amount, float duration = 0f, bool hide = false);    // positive
    void OnDeath();
}