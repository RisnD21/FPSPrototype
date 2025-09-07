using UnityEngine;

public class ListenerAttacher : MonoBehaviour
{
    [SerializeField] Transform target;  // 指向玩家；玩家死/換角時更新
    [SerializeField] Vector3 offset = Vector3.zero;    // 需要的相對位置（可留 0）

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        // target 暫時為 null 時，保留上次位置即可，聲場不會炸
    }

    public void SetTarget(Transform t) => target = t;
}