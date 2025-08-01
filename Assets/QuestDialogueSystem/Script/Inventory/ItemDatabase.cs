using UnityEngine;
using System.Collections.Generic;

namespace QuestDialogueSystem
{
    public static class ItemDatabase
    {
        static Dictionary<string, ItemData> itemMap = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitOnStart()
        {
            Initialize();
        }

        public static void Initialize()
        {
            ResetDictionary();
            LoadAll();
        }

        static void LoadAll()
        {
            var items = Resources.LoadAll<ItemData>("Items");
            foreach (var item in items)
            {
                if(!itemMap.ContainsKey(item.itemID))
                {
                    itemMap[item.itemID] = item;
                    Debug.Log($"[ItemDatabase] register item: {item.itemID} {item.itemName}");
                } else 
                {
                    Debug.LogWarning("[ItemDatabase] Duplicate Item ID detected" + item.itemID);
                }
            }
        }

        static void ResetDictionary()
        {
            itemMap.Clear();
        }

        public static bool TryGetItemData(string id, out ItemData itemData)
        {
            if(itemMap.ContainsKey(id))
            {
                itemData = itemMap[id];
                return true;
            }
            else
            {
                itemData = null;
                Debug.LogWarning("[ItemDatabase] ID Invalid");
                return false;
            }
        }
    }
}

