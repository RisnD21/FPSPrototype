using Unity.VisualScripting;
using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/Item/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemID;
        public string itemName;
        public Sprite itemIcon;
        [TextArea(3,5)]
        public string description;
        public int maxStack;
        public string itemType;
        public ItemAction itemAction;
        public bool HasAction() => itemAction != null;
        public bool CanDrop;

        [TextArea(3,5)]
        public string msgOnDrop;
        

        public bool TryUse(UseContext useContext, InventorySlot slot)
        {
            if(itemAction == false) return false;
            return itemAction.TryUse(useContext, slot);
        }

        public bool TryUse(UseContext useContext, ItemData item)
        {
            if(itemAction == false) return false;
            return itemAction.TryUse(useContext, item);
        }

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

        public override string ToString()
        {
            return itemName;
        }
    }
}