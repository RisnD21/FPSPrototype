using UnityEngine;
using System.Collections.Generic;

//Draw line based on given parameters
public class RaycastShotController : MonoBehaviour
{
    public static RaycastShotController Instance {get; private set;}

    [SerializeField] LineRenderer handgunShotPrefab;

    float trailRemainTime = 0.1f;
    float trailSpeed = 200.0f;
    List<ActiveTrail> m_ActiveTrails;
    class ActiveTrail
    {
        public LineRenderer renderer;
        public Vector3 direction;
        public float remainingTime;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        m_ActiveTrails = new(); 
    }

    public void SetGunType(string type)
    {
        switch(type)
        {
            case "handgun":
                if(!PoolSystem.Instance.Contains(handgunShotPrefab))
                    PoolSystem.Instance.InitPool(handgunShotPrefab, 10);
                break;

            default:
                break;
        }
    }

    public void AddTrail(Vector3 startPos, Vector3 endPos)
    {
        var pos = new Vector3[] { startPos, endPos };
        var trail = PoolSystem.Instance.GetInstance<LineRenderer>(handgunShotPrefab);

        trail.gameObject.SetActive(true);
        trail.positionCount = 2;
        trail.SetPositions(pos);
        m_ActiveTrails.Add(new ActiveTrail()
        {
            renderer = trail,
            remainingTime = trailRemainTime,
            direction = (pos[1] - pos[0]).normalized
        });
    }
    void Update()
    {
        DrawLine();
    }

    void DrawLine()
    {
        Vector3[] pos = new Vector3[2];

        for (int i = 0; i < m_ActiveTrails.Count; ++i)
        {
            var activeTrail = m_ActiveTrails[i];
            
            activeTrail.renderer.GetPositions(pos);
            activeTrail.remainingTime -= Time.deltaTime;

            pos[0] += Time.deltaTime * trailSpeed * activeTrail.direction;

            var fromHeadToTail = pos[1] - pos[0];
            float dot = Vector3.Dot(fromHeadToTail.normalized, activeTrail.direction);
        
            if (dot < 0f || activeTrail.remainingTime <= 0.0f)
            {
                activeTrail.renderer.gameObject.SetActive(false);
                m_ActiveTrails.RemoveAt(i);
                i--;
            } else
            {
                activeTrail.renderer.SetPositions(pos);
            }
        }
    }
}