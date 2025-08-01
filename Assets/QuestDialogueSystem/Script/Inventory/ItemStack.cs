using System;
using UnityEngine;

namespace QuestDialogueSystem
{
    [Serializable]
    public struct ItemStack
    {
        public ItemData Item{get;}
        public int Count{get;}

        public static readonly ItemStack Empty = new((ItemData)null, 0);
        public int Max => IsEmpty? 0 : Item.maxStack;
        public bool IsEmpty => Item == null || Count <= 0;

        public ItemStack(ItemData item, int count)
        {
            if (count < 0) throw new ArgumentException("[ItemStack] Count must be >= 0");

            Item = item;
            Count = count;
        }

        public ItemStack(string itemID, int count)
            : this(GetItemDataFromID(itemID), count){}

        static ItemData GetItemDataFromID(string itemID)
        {
            if (!ItemDatabase.TryGetItemData(itemID, out var item))
                throw new ArgumentException("[ItemStack] Invalid ID: " + itemID);
            return item;
        }
   
        public ItemStack WithCount(int newCount) => new(Item, newCount);

        public override string ToString()
        {
            if(Item == null) return "Empty Stack";
            return $"{Count} x {Item.itemName}";
        }
    }
}