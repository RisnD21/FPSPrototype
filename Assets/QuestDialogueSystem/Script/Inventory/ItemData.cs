using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemID;
        public string itemName;
        public Sprite itemIcon;
        [TextArea(3,5)]
        public string description;
        public int maxStack;
        public string itemType;

        public override bool Equals(object obj)
        {            
            if (obj != null && obj is ItemData other)
            {
                return other.itemID == itemID;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            if (itemID == null) return 0;
            return itemID.GetHashCode();
        }
    }
}