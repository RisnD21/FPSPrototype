using System.Collections.Generic;
using UnityEngine;

//記錄物件初始的相對位置，並使其不受 parents rotation 影響
//已修正：
//1. 加入 tolerance，超過誤差才調整 target
//2. Update 改為 LateUpdate
//3. rotation != data.rotation 改為 Quaternion.Angle(target.rotation, data.rotation) > tolerance
//4. TransformData 無需頻繁改動，改為 Structure


public class FixRelatedPos : MonoBehaviour
{
    
    [SerializeField] List<Transform> targets;
    [SerializeField] float tolerance = 0.01f;

    Dictionary<Transform, TransformData> index;

    public struct TransformData 
    {
        public Vector3 relatedPosition;
        public Quaternion rotation;
    }

    void Awake()
    {
        index = new();
        BuildIndex();
    }

    void BuildIndex()
    {
        foreach (var target in targets)
        {
            TransformData data = new()
            {
                relatedPosition = target.position - transform.position,
                rotation = target.transform.rotation
            };
            index[target] = data;
        }
    }

    void OverrideTransform(Transform target)
    {
        if(!index.TryGetValue(target, out var data))
        {
            Debug.LogError("[FixTransform] data corruption, key not found");
            return;
        }

        var newRelatedPosition = target.position - transform.position;
        var delta = newRelatedPosition - data.relatedPosition;
        var angle = Quaternion.Angle(target.rotation, data.rotation);
        if(delta.magnitude > tolerance|| angle > tolerance)
            target.SetPositionAndRotation(data.relatedPosition + transform.position, data.rotation);
    }

    void UpdateTargetsTransform()
    {
        foreach (var target in targets)
        {
            OverrideTransform(target);
        }
    }

    void LateUpdate()
    {
        UpdateTargetsTransform();
    }
}
