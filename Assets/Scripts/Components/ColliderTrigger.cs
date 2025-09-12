using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent trigger;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) 
        {
            trigger?.Invoke();
        }
    }
}