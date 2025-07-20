using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector2 deadZoneSize = new(12f, 7f);
    public float lockOnPlayerRangeIn = 5f;
    public float lockOnPlayerRangeOut = 6f;
    public float maxSmoothSpeed = 5f;
    public float minSmoothSpeed = 2f;
    public float tolerance = 0.1f;
    Vector3 lastFocus;
    float maxMouseDistance;
    bool lockOnPlayer; 

    void Start()
    {
        CalculateMaxMouseDistance();
    }

    void CalculateMaxMouseDistance()
    {
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;
        maxMouseDistance = new Vector2(camWidth, camHeight).magnitude / 2f;
    }

// 1. 角色在攝影機的 Deadzone 中：
//     1. 若滑鼠指標位於角色附近：攝影機滑向角色
//     2. 否則攝影機滑向滑鼠指標，該距離上限為 Deadzone 之最短邊界
// 2. 否則：
//     1. 攝影機滑向角色
// 3. 攝影機中心距離目標越遠滑動速度越快

    Vector3 GetFocus()
    {
        Vector3 delta = Vector3.zero;

        Vector3 playerPos = player.position;
        Vector3 distanceToPlayer = playerPos - transform.position;

        bool outsideX = Mathf.Abs(distanceToPlayer.x) > deadZoneSize.x / 2f;
        bool outsideY = Mathf.Abs(distanceToPlayer.y) > deadZoneSize.y / 2f;

        if (outsideX || outsideY) return playerPos;

        // 玩家在 deadzone 內，若滑鼠距離玩家過遠，則聚焦在滑鼠上
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 playerToMouse = mouseWorldPos - playerPos;

        // 鎖定開關（hysteresis）
        if (playerToMouse.magnitude < lockOnPlayerRangeIn) lockOnPlayer = true;
        if (playerToMouse.magnitude > lockOnPlayerRangeOut) lockOnPlayer = false;

        if(!lockOnPlayer)
        {
            float maxOffset = Mathf.Min(deadZoneSize.x, deadZoneSize.y) /2;

            if (playerToMouse.magnitude > maxOffset)
            {
                delta = maxOffset * playerToMouse.normalized;
            } else delta = playerToMouse;
        }

        return playerPos + delta;
    }

    void ShiftTo(Vector3 target)
    {
        target.z = transform.position.z;
        Vector3 newPos;

        // Debug.Log($"distance between player & target = {(target-player.transform.position).magnitude}");
        // Debug.Log($"Target = " + target);
        //result in 1, 6.00x if we focus on far distance

        float distanceRatio = 
        Mathf.Clamp01((target-transform.position).magnitude / maxMouseDistance);

        float dynamicSmoothSpeed = 
        Mathf.Lerp(minSmoothSpeed, maxSmoothSpeed, distanceRatio);

        newPos = Vector3.Lerp(transform.position, target, Time.deltaTime * dynamicSmoothSpeed);

        if((newPos - target).magnitude < tolerance/2) newPos = target;
            
        newPos.z = transform.position.z;
        transform.position = newPos;
        lastFocus = target;
    }

    void LateUpdate()
    {
        if (CameraShaker.Instance == null || CameraShaker.Instance.hasControl) return;
        Vector3 newFocus = GetFocus();
        if ((transform.position - newFocus).magnitude > tolerance) ShiftTo(newFocus);
    }
}