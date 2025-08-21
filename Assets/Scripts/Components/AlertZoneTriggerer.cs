using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AlertZoneTriggerer : MonoBehaviour
{
    [SerializeField] GameObject aiAgentObject;
    public LayerMask NPCLayer;
    AIAgent aiAgent;

    CircleCollider2D col;
    void Awake()
    {
        aiAgent = aiAgentObject.GetComponent<AIAgent>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, col.radius, NPCLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out AIAgent agent) && agent != aiAgent)
                aiAgent.AddNearbyAllies(agent);                
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) aiAgent.SubscribeToPlayerNoiseSpeaker();
        else if (collision.TryGetComponent<AIAgent>(out var agent) && agent != aiAgent)
        {
            aiAgent.AddNearbyAllies(agent);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) aiAgent.UnsubscribeToPlayerNoiseSpeaker();
        if(collision.TryGetComponent<AIAgent>(out var agent))
            aiAgent.RemoveNearbyAllies(agent);
    }


}