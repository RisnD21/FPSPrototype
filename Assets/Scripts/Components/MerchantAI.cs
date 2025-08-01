using UnityEngine;
using System.Collections;

public class MerchantAI : MonoBehaviour, IInteractable
{    
    public Transform player;
    public LayerMask obstacleMask;
    public float viewAngle = 120.0f;

    bool isDebugMode;
    Coroutine currentRoutine; // 這是關鍵

    public IEnumerator LookAt(Vector3 point, float duration = 0.5f)
    {
        if (isDebugMode) Debug.Log($"{gameObject.name} is looking at {point}");

        Vector3 direction = point - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, targetAngle);
        
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        currentRoutine = null;
    }

    public bool IsPlayerInSight()
    {
        if(player == null) return false;

        Vector2 dirToPlayer = player.position - transform.position;
        float angle = Vector2.Angle(dirToPlayer, transform.up); //face up
        if (viewAngle/2f < angle) return false;

        float distanceToPlayer = dirToPlayer.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);
        if (hit.collider != null) return false;

        if(distanceToPlayer > 20f) return false;

        return true;
    }

    void Update()
    {
        if(IsPlayerInSight() && currentRoutine == null)
        {
            currentRoutine = StartCoroutine(LookAt(player.position, 0.3f));
        }
    }

    public void Interact()
    {
        Debug.Log("(Remain Silent)");
    }
}
