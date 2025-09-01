using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(PolygonCollider2D))]
public class ShadowCaster2DFromCollider : MonoBehaviour
{
    [Tooltip("自動在 OnValidate/Reset 時同步Collider形狀到ShadowCaster")]
    public bool autoSyncInEditor = true;

    [Tooltip("若場景載入時希望自動套用一次，可打勾")]
    public bool syncOnEnable = false;

    [Tooltip("從Collider取用的路徑索引（多路徑Collider只支援擇一）")]
    [Min(0)] public int colliderPathIndex = 0;

    ShadowCaster2D caster;
    PolygonCollider2D col;

#if UNITY_EDITOR    
    void Reset() 
    { 
        TryCache(); 
        if (autoSyncInEditor) SyncNow();
    }
    void OnValidate()
    {
        TryCache(); 
        if (autoSyncInEditor) SyncNow();
    }
#endif 

    void OnEnable(){ TryCache(); if (syncOnEnable) SyncNow(); }

    void TryCache()
    {
        if (!col) col = GetComponent<PolygonCollider2D>();
        if (!caster) caster = GetComponent<ShadowCaster2D>();
        if (!caster) caster = gameObject.AddComponent<ShadowCaster2D>();
    }

    [ContextMenu("Sync Collider → ShadowCaster2D")]
    public void SyncNow()
    {
        if (!col) return;
        if (!caster) caster = gameObject.AddComponent<ShadowCaster2D>();

        int pathCount = col.pathCount;
        if (pathCount == 0) return;

        int idx = Mathf.Clamp(colliderPathIndex, 0, pathCount - 1);
        Vector2[] path = col.GetPath(idx);

        ApplyShapeToShadowCaster(caster, path);
    }

#if UNITY_EDITOR
    static void ApplyShapeToShadowCaster(ShadowCaster2D target, Vector2[] path)
    {
        var so = new SerializedObject(target);

        // 關掉由Renderer外形推導，改用自訂多邊形
        so.FindProperty("m_UseRendererSilhouette").boolValue = false;
        // 是否自投影（通常關掉較直觀）
        so.FindProperty("m_SelfShadows").boolValue = false;

        // 設定形狀路徑（內部序列化欄位）
        var shapeProp = so.FindProperty("m_ShapePath");
        shapeProp.arraySize = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            shapeProp.GetArrayElementAtIndex(i).vector3Value = new Vector3(path[i].x, path[i].y, 0f);
        }

        // 更新形狀雜湊值（避免偶發不重建）
        var hashProp = so.FindProperty("m_ShapePathHash");
        hashProp.intValue = Random.Range(int.MinValue, int.MaxValue);

        so.ApplyModifiedPropertiesWithoutUndo();

        // 觸發重建
        EditorUtility.SetDirty(target);
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
#else
    static void ApplyShapeToShadowCaster(ShadowCaster2D target, Vector2[] path) { }
#endif
}