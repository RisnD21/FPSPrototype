//Those who cares health value should implement this
public interface IHealthListener
{
    void OnHealthChanged(int current, int max);
    void OnDamaged(int amount);   // positive
    void OnHealed(int amount);    // positive
    void OnDeath();
}