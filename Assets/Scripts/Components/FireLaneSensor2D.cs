using UnityEngine;

public class FireLaneSensor2D : MonoBehaviour
{
    [Header("Ballistics")]
    [SerializeField] Transform muzzle; //For gizmo drawing
    [SerializeField] float range = 12f;

    [Tooltip("準心擴散 (Full Degree)")]
    [SerializeField] float spreadDeg = 6f;
    [Range(1, 15)] public int sampleRays = 7;

    [Header("Layers")]
    [SerializeField] LayerMask allyMask;

    static readonly RaycastHit2D[] _hits = new RaycastHit2D[12];

    bool isDebugMode = false;
    public bool HasFriendlyInFireLane(Vector2 origin, Vector2 up)
    {
        if(up.sqrMagnitude <= 1e-6f) Debug.LogError("direction is zero");
        up = up.normalized;
        
        int n = Mathf.Max(1, sampleRays);

        for (int i = 0; i < n; i++)
        { 
            float t = (n == 1) ? 0f : (i / (float)(n - 1)) * 2f - 1f; // [-1,1]
            float off = t * (spreadDeg * 0.5f);
            Vector2 dir = Rotate(up, off);

            int count = Physics2D.RaycastNonAlloc(origin, dir, _hits, range, allyMask);

            if (count > 0) 
            {
                if(isDebugMode) Debug.Log($"{gameObject.name} sees {_hits[0].collider.gameObject.name} in front");
                return true;
            } 
        }

        return false;
    }

    Vector2 Rotate(Vector2 v, float deg)
    {
        float r = deg * Mathf.Deg2Rad; float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }

    void OnDrawGizmosSelected()
    {
        if (!muzzle) return;
        Vector2 o = muzzle.position;
        Vector2 fwd = muzzle.right.normalized;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < sampleRays; i++)
        {
            float t = (sampleRays == 1) ? 0f : (i / (float)(sampleRays - 1)) * 2f - 1f;
            float off = t * (spreadDeg * 0.5f);

            Vector2 d = Rotate(fwd, off);
            Gizmos.DrawLine(o, o + d * range);
        }
    }
}
