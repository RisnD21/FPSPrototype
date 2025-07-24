using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance {get; private set;}

    Dictionary<string, Pickable> itemIndex;

    [System.Serializable]
    public class ItemEntry
    {
        public string id;
        public Pickable prefab;
    }
    [SerializeField] List<ItemEntry> itemList = new();


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        BuildIndex();
    }

    void BuildIndex()
    {
        itemIndex = new();
        foreach(var entry in itemList)
        {
            itemIndex[entry.id] = entry.prefab;
        }
    }

    public void Initialize()
    {
        foreach (var item in itemIndex)
        {
            PoolSystem.Instance.InitPool(item.Value, 3);
        }
    }

    public void SpawnItem(string id, Vector3 position)
    {
        if(!itemIndex.TryGetValue(id, out var item))
        {
            Debug.LogWarning("[SpawnItemManager] item ID not found, do nothing");
            return;
        }

        var itemPrefab = PoolSystem.Instance.GetInstance<Pickable>(item);
        itemPrefab.transform.position = position;
        itemPrefab.gameObject.SetActive(true);
    }
}