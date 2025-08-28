using QuestDialogueSystem;
using UnityEngine;
using System.Collections.Generic;

public class QuickSlotsManager : MonoBehaviour 
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] List<QuickSlotUI> quickSlots;
    [SerializeField] PlayerUseItemSystem playerUseItemSystem;

    void OnEnable()
    {
        PlayerInput.SetQuickSlot += SetQuickSlot;
        PlayerInput.UseQuickSlot += UseQuickSlot;
        
        Locator.Inventory.OnItemAdd += UpdateQuickSlots;
        Locator.Inventory.OnItemRemove += UpdateQuickSlots;
    }

    void OnDisable()
    {
        PlayerInput.SetQuickSlot -= SetQuickSlot;
        PlayerInput.UseQuickSlot -= UseQuickSlot;

        Locator.Inventory.OnItemAdd -= UpdateQuickSlots;
        Locator.Inventory.OnItemRemove -= UpdateQuickSlots;
    }

    void UpdateQuickSlots(ItemStack _)
    {
        SyncQuickSlots();
    }

    void SyncQuickSlots()
    {
        foreach (var slot in quickSlots) slot.Refresh();
    }


    void SetQuickSlot(int index)
    {
        if(index > quickSlots.Count - 1) return;

        var targetSlot = quickSlots[index];
        var item = inventoryUI.itemSelected;
        
        if(item != null && item.HasAction() && Locator.Inventory.Count(item) != 0 && item.itemType != "Shield")
        {
            foreach (var slot in quickSlots)
            {
                if(!slot.IsEmpty && slot.Item.Equals(item))
                {
                    slot.ClearSlot();
                }
            }

            targetSlot.SetSlot(item);
        } else targetSlot.ClearSlot();
    }

    void UseQuickSlot(int index)
    {
        if(index > quickSlots.Count - 1) return;

        var targetSlot = quickSlots[index];
        var item = targetSlot.Item;

        playerUseItemSystem.UseItem(item);
    }
}