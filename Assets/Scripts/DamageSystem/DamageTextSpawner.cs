using UnityEngine;

//This should be placed on damagedTextPos gameobject
public class DamageTextSpawner : MonoBehaviour, IHealthListener
{

    [SerializeField] Color healColor = Color.green;
    [SerializeField] Color damageColor = Color.red;

    public void OnHealthChanged(int current, int max, bool hide = false){}
    public void OnDamaged(int amount, float duration = 0f, bool hide = false)
    {
        if(!hide) SpawnText(amount, damageColor);
    }
    public void OnHealed(int amount, float duration = 0f, bool hide = false)
    {
        if(!hide) SpawnText(amount, healColor);
    }

    public void OnDeath(){}

    void SpawnText(int value, Color color)
    {
        DamageTextManager.Instance.SpawnText(value, color, transform.position);
    }
}