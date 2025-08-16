using UnityEngine;
using System.Collections.Generic;

namespace QuestDialogueSystem
{
    public static class ItemDatabase
    {
        static Dictionary<string, ItemData> itemMap = new();
        static Dictionary<string, List<ItemData>> typeMap = new();

        static bool isDebugMode = false;


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
                    if (isDebugMode) Debug.Log($"[ItemDatabase] register item: {item.itemID} {item.itemName}");

                    if(!typeMap.TryGetValue(item.itemType, out var list))
                    {
                        list = new();
                        typeMap[item.itemType] = list;
                    }
                    list.Add(item);
                } else 
                {
                    Debug.LogWarning("[ItemDatabase] Duplicate Item ID detected " + item.itemID);
                }
            }
        }

        static void ResetDictionary()
        {
            itemMap.Clear();
            typeMap.Clear();
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

        public static bool TryGetItemsByType(string type, out List<ItemData> itemDatas)
        {
            if(typeMap.ContainsKey(type))
            {
                itemDatas = typeMap[type];
                return true;
            }
            else
            {
                itemDatas = null;
                Debug.LogWarning("[ItemDatabase] Type Invalid");
                return false;
            }
        }
    }
}

