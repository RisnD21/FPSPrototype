
using QuestDialogueSystem;
using UnityEngine;

public abstract class ItemAction : ScriptableObject 
{
    public abstract bool TryUse(UseContext useContext, InventorySlot slot = null);
}