using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestDialogueSystem;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    //掌管所有 slot prefabs
    //訂閱 inventory 能在物件更新時更新對應欄位
    //隨 inventory ui 開關而作業 => 主動索取當前 slots

    [SerializeField] Transform slotParent;
    [SerializeField] Transform slotPrefab;
    [SerializeField] int slotsCount = 24;

    List<SlotUI> slots;

    void Awake()
    {
        slots = new();

        for(int i = 0; i < slotsCount; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, slotParent).GetComponent<SlotUI>();
            slot.gameObject.SetActive(false);
            slots.Add(slot);
        }
    }

    public void Initialize()
    {
        SyncInventory();
    }

    void OnEnable()
    {
        Locator.Inventory.OnItemAdd += UpdateSlot;
        Locator.Inventory.OnItemRemove += UpdateSlot;
    }

    void OnDisable()
    {
        Locator.Inventory.OnItemAdd -= UpdateSlot;
        Locator.Inventory.OnItemRemove -= UpdateSlot;
    }

    void UpdateSlot(ItemStack _)
    {
        SyncInventory();
    }

    void SyncInventory()
    {
        if(Locator.Inventory.Slots.Count != slotsCount)
        {
            Debug.LogError("[InventoryUI] Inventory slots amount mismatch");
            return;
        }

        for(int i = 0; i < slotsCount; i++)
        {
            slots[i].SetSlot(Locator.Inventory.Slots[i]);
        }
    }
}
