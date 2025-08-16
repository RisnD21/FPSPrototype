using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AlertZoneTriggerer : MonoBehaviour
{
    [SerializeField] GameObject aiAgentObject;
    public LayerMask layerMask;
    AIAgent aiAgent;

    CircleCollider2D col;
    void Awake()
    {
        aiAgent = aiAgentObject.GetComponent<AIAgent>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, col.radius, layerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out AIAgent agent))
                aiAgent.AddNearbyAllies(agent);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) aiAgent.SenseSomething();
        else if (collision.TryGetComponent<AIAgent>(out var agent))
        {
            agent.AddNearbyAllies(agent);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<AIAgent>(out var agent))
            agent.RemoveNearbyAllies(agent);
    }
}