using UnityEngine;

public class DeathEffect : MonoBehaviour, IHealthListener
{
    [SerializeField] GameObject target;
    [SerializeField] float delay = 0.1f; // 讓其他 listener 有時間處理

    public void OnHealthChanged(int current, int max){}
    public void OnDamaged(int amount){}
    public void OnHealed(int amount){}
    public void OnDeath()
    {
        if(target == null) return;
        Debug.Log("Destroying " + target.name);
        Destroy(target, delay);
    }
}