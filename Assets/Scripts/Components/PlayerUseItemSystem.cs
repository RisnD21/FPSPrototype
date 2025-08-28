using UnityEngine;
using QuestDialogueSystem;

public class PlayerUseItemSystem : MonoBehaviour
{
    UseContext ctx;
    public void Start()
    {
        AssembleUseContext();
    }

    void AssembleUseContext()
    {
        ctx = new(gameObject, Locator.Inventory);
    }

    void OnEnable()
    {
        InventoryUI.OnItemUse += UseItem;
    }

    void OnDisable()
    {
        InventoryUI.OnItemUse -= UseItem;
    }

    void UseItem(InventorySlot slot)
    {
        if(slot == null)
        {
            Debug.LogWarning("[PlayerUseItemSystem] Missing slot ");
            return;
        } else if(slot.stack.IsEmpty)
        {
            Debug.LogWarning("[PlayerUseItemSystem] Attempting to access empty slot");
            return;
        }
        slot.stack.Item.TryUse(ctx, slot);
    }

    public void UseItem(ItemData item)
    {
        if(item == null)
        {
            Debug.LogWarning("[PlayerUseItemSystem] Missing item ");
            return;
        } 
        item.TryUse(ctx, item);
    }
}