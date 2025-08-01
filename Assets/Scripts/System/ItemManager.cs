using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using QuestDialogueSystem;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemEntry
    {
        public string id;              // 物品識別用
        public Pickable prefab;        // 世界空間的可拾取物件 Prefab
        public Sprite icon;            // UI 上對應的圖示
        public int poolSize = 5;       // PoolSystem 要初始化的大小
    }

    public static ItemManager Instance { get; private set; }

    [Header("Item 設定")]
    [SerializeField] private List<ItemEntry> items;

    [Header("UI 飛行圖示")]
    [SerializeField] private RectTransform uiSpritePrefab;    // 預先製作好的 Image Prefab 
    [SerializeField] private Transform uiSpriteParent;    // Image Prefab 之 parent
    [SerializeField] private Canvas uiCanvas;                 // 接受圖示的 Canvas
    [SerializeField] private RectTransform targetIcon;        // 飛往的目標 Icon
    [SerializeField] private float flyDuration = 0.6f;        // 飛行時間
    [SerializeField] private Ease flyEase = Ease.InOutQuad;   // 飛行曲線

    // id → 物品設定
    private Dictionary<string, ItemEntry> entryMap;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void Initialize()
    {
        BuildIndexAndInitPools();
    }

    private void BuildIndexAndInitPools()
    {
        entryMap = new Dictionary<string, ItemEntry>();

        // 先算總共要幾個 UI 項目
        int totalUI = 0;
        foreach (var e in items)
        {
            entryMap[e.id] = e;
            totalUI += e.poolSize;

            // 初始化世界物件池
            PoolSystem.Instance.InitPool(e.prefab, e.poolSize);
        }

        // 初始化 UI 圖示池（共用同一個 prefab)
        PoolSystem.Instance.InitPool(uiSpritePrefab, totalUI, uiSpriteParent);
    }

    /// <summary>
    /// 在世界中生成一個可拾取物件
    /// </summary>
    public void SpawnPickable(ItemStack stack, Vector3 position)
    {
        string id = stack.Item.itemID;

        if (!entryMap.ContainsKey(id))
        {
            Debug.LogError($"[ItemManager] 找不到 id={id}");
            return;
        }

        var entry = entryMap[id];
        // 從 PoolSystem 拿一個新的實例
        var inst = PoolSystem.Instance.GetInstance<Pickable>(entry.prefab);
        inst.stack = stack;
        inst.Manager = this;
        inst.transform.position = position;
        return;
    }

    /// <summary>
    /// 玩家真正「撿到」物品時只要呼這裡
    /// </summary>
    public bool TryPickItem(ItemStack stack, Vector3 worldPosition)
    {
        string id = stack.Item.itemID;

        if (!entryMap.ContainsKey(id))
        {
            Debug.LogError($"[ItemManager] 找不到 id={id}");
            return false;
        }

        if(Locator.Inventory.TryAdd(stack, out _))
        {
            SpawnUIIcon(id, worldPosition);
            return true;
        }else return false;
        // 呼叫 UI 飛行效果
        
        // TODO: 增加背包、音效、數量…等邏輯
        
    }

    //--------------- 私有 Method ---------------//
    private void SpawnUIIcon(string id, Vector3 worldPos)
    {
        var entry = entryMap[id];

        var uiInst = PoolSystem.Instance.GetInstance<RectTransform>(uiSpritePrefab);

        var img = uiInst.GetComponent<Image>();
        if (img != null) img.sprite = entry.icon;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            uiCanvas.transform as RectTransform,
            screenPos,
            uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
            out Vector3 worldPointOnCanvas
        );

        uiInst.position = worldPointOnCanvas;
        uiInst.gameObject.SetActive(true);

        uiInst.DOMove(targetIcon.position, flyDuration)
            .SetEase(flyEase)
            .OnComplete(() =>
            {
                uiInst.gameObject.SetActive(false);
            });
    }

}
