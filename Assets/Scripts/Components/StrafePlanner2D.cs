using UnityEngine;

[RequireComponent(typeof(FireLaneSensor2D))]
public class StrafePlanner2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float strafeStep = 0.6f;
    [SerializeField] int strafeMaxSteps = 3;

    [Header("Behavior")]
    [SerializeField] float recheckCooldown = 0.25f;

    float _nextAllowedCheckTime;

    [Header("Tool")]
    [SerializeField] FireLaneSensor2D fireLaneSensor;


    [SerializeField] LayerMask blockerMask;   // obstacle | ally
    [SerializeField] float occupyRadius = 0.25f;

    Rigidbody2D selfRb;
    ContactFilter2D overlapFilter;
    readonly Collider2D[] _buf = new Collider2D[12];

    bool isDebugMode = false;

    void Awake() {
        selfRb = GetComponent<Rigidbody2D>();

        overlapFilter = new ContactFilter2D {
            useTriggers = false    // 忽略 Trigger，避免假阻擋
        };

        overlapFilter.SetLayerMask(blockerMask);
    }

    public Vector2? GetStrafePos(Vector2 origin, Vector2 muzzlePos, Vector2 forward)
    {
        if (Time.time < _nextAllowedCheckTime) return null; // 還在冷卻：視為「正在補位」

        if(isDebugMode) Debug.Log($"{gameObject.name} is trying to strafe");
        Vector2 left = new(-forward.y, forward.x);
        Vector2 right = -left;

        if (TrySide(origin, left, forward, muzzlePos, out var pos) || TrySide(origin, right, forward, muzzlePos, out pos))
        {
            _nextAllowedCheckTime = Time.time + recheckCooldown;
            return pos;
        }
        return null;
    }

    bool TrySide(Vector2 origin, Vector2 side, Vector2 forward, Vector2 muzzlePos, out Vector2 pos)
    {
        pos = origin;

        for (int i = 1; i <= strafeMaxSteps; i++)
        {
            Vector2 cand = origin + side.normalized * (i * strafeStep);
            Vector2 newMuzzlePos = muzzlePos + side.normalized * (i * strafeStep);

            if(isDebugMode) Debug.Log($"[{gameObject.name}] Testing if should move to " + cand);

            if (IsBlockedByOthers(cand)) 
            {
                if(isDebugMode) Debug.Log($"[{gameObject.name}] Fail: Overlapping with obstacle / ally");
                return false;
            }

            if (fireLaneSensor.HasFriendlyInFireLane(newMuzzlePos, forward))
            {
                if(isDebugMode) Debug.Log($"[{gameObject.name}] Fail: Desire pos will still hit ally");
                continue;
            }

            pos = cand;
            if(isDebugMode) Debug.Log($"[{gameObject.name}] Success: Found better pos");
            return true;
        }
        if(isDebugMode) Debug.Log($"[{gameObject.name}] Fail: All solutions are invalid");
        return false;
    }

    // cand 這個候選點是否被「別人」佔住
    bool IsBlockedByOthers(Vector2 cand) {
        int count = Physics2D.OverlapCircle(cand, occupyRadius, overlapFilter, _buf);

        for (int i = 0; i < count; i++) {
            var col = _buf[i];
            if (!col) continue;

            // 忽略「自己」的所有碰撞體
            if (col.attachedRigidbody && col.attachedRigidbody == selfRb) continue;
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            if(isDebugMode) Debug.Log("[{gameObject.name}] testing obstacle / ally");
            return true; // 有別人佔位
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector2 forward = transform.up;

        Vector2 left = new(-forward.y, forward.x);
        Vector2 right = -left;

        Gizmos.DrawLine(transform.position, (Vector2) transform.position + left.normalized * (strafeMaxSteps * strafeStep));
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + right.normalized * (strafeMaxSteps * strafeStep));
    }
}