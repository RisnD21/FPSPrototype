using UnityEngine;

public class DamageForwarder : MonoBehaviour 
{
    [SerializeField] DamagePipeline pipeline;  // 指到角色的根 Pipeline

    void Awake()
    {
        if (pipeline == null) pipeline = GetComponentInParent<DamagePipeline>();
    }

    public void ApplyHit(int amount)
    {
        if (pipeline == null || amount <= 0) return;
        pipeline.TakeDamage(amount);
    }
}