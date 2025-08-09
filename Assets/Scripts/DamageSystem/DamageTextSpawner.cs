using UnityEngine;

//This should be placed on damagedTextPos gameobject
public class DamageTextSpawner : MonoBehaviour, IHealthListener
{
    public void OnHealthChanged(int current, int max){}
    public void OnDamaged(int amount)
    {
        SpawnText(-amount);
    }
    public void OnHealed(int amount)
    {
        SpawnText(amount);
    }

    public void OnDeath(){}

    void SpawnText(int value)
    {
        DamageTextManager.Instance.SpawnText(value, transform.position);
    }
}