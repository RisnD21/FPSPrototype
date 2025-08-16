using UnityEngine;

public class OnHitBehavior : MonoBehaviour, IHealthListener
{
    [SerializeField] GameObject target;
    [SerializeField] float delay = 0.1f; // 讓其他 listener 有時間處理


    [SerializeField] bool isNPC;
    [SerializeField] GameObject aIAgent;
    AIAgent agent;

    void Awake()
    {
        if(isNPC) agent = aIAgent.GetComponent<AIAgent>();
    }

    public void OnHealthChanged(int current, int max, bool hide = false){}
    public void OnDamaged(int amount, float duration = 0f, bool hide = false)
    {
        if(isNPC) agent.OnHit();
    }
    public void OnHealed(int amount, float duration = 0f, bool hide = false){}
    public void OnDeath()
    {
        if(target == null) return;
        Debug.Log("Destroying " + target.name);
        Destroy(target, delay);
    }
}