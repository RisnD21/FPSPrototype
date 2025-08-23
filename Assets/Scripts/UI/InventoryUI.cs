using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour
{
    //掌管所有 slot prefabs
    //訂閱 inventory 能在物件更新時更新對應欄位
    //隨 inventory ui 開關而作業 => 主動索取當前 slots

    [SerializeField] Transform slotParent;
    [SerializeField] Transform slotPrefab;
    [SerializeField] int slotsCount = 24;
    List<SlotUI> slots;

    [SerializeField] GameObject descriptionPanel;
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] GameObject itemMenu;
    [SerializeField] GameObject blocker;
    [SerializeField] Button useItemButton;
    [SerializeField] Button destroyItemButton;
    
    [SerializeField] Vector3 itemMenuOffset;
    [SerializeField] Vector3 descriptionPanelOffset;

    public static event Action<InventorySlot> OnItemUse;
    InventorySlot currentSlot;
    void Awake()
    {
        transform.localPosition = Vector3.zero;

        slots = new();

        for(int i = 0; i < slotsCount; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, slotParent).GetComponent<SlotUI>();
            slot.InitializeSlot(this);
            slot.gameObject.SetActive(false);
            slots.Add(slot);
        }

        descriptionPanel.SetActive(false);
        itemMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void Initialize()
    {
        SyncInventory();
    }

    void OnEnable()
    {
        Locator.Inventory.OnItemAdd += UpdateSlot;
        Locator.Inventory.OnItemRemove += UpdateSlot;

        SyncInventory();
    }

    void OnDisable()
    {
        Locator.Inventory.OnItemAdd -= UpdateSlot;
        Locator.Inventory.OnItemRemove -= UpdateSlot;
        CloseItemMenu();
        HideDescriptionPanel();
    }

    void UpdateSlot(ItemStack _)
    {
        SyncInventory();
    }

    void SyncInventory()
    {
        if (Locator.Inventory.Slots.Count != slotsCount)
        {
            Debug.LogError("[InventoryUI] Inventory slots amount mismatch");
            return;
        }

        for(int i = 0; i < slotsCount; i++)
        {
            slots[i].SetSlot(Locator.Inventory.Slots[i]);
        }
    }

    public void OpenItemMenu(InventorySlot slot, Vector3 position)
    {
        currentSlot = slot;
        
        if (currentSlot.stack.Item.HasAction()) 
        {
            useItemButton.onClick.RemoveAllListeners();
            useItemButton.onClick.AddListener(UseItem);
            useItemButton.gameObject.SetActive(true);
        } else useItemButton.gameObject.SetActive(false);

        destroyItemButton.onClick.RemoveAllListeners();
        destroyItemButton.onClick.AddListener(DestroyItem);

        itemMenu.transform.position = position + itemMenuOffset;
        itemMenu.SetActive(true);
        blocker.SetActive(true);
    }

    void UseItem()
    {
        OnItemUse?.Invoke(currentSlot);
        CloseItemMenu();
    }

    void DestroyItem()
    {
        int toRemove = currentSlot.stack.Count;
        Locator.Inventory.TryRemoveFromSlot(currentSlot.stack, currentSlot, ref toRemove);
        CloseItemMenu();
    }

    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void ShowDescriptionPanel(InventorySlot slot, Vector3 position)
    {
        description.text = slot.stack.Item.description;
        descriptionPanel.transform.position = position + descriptionPanelOffset;
        descriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {   
        descriptionPanel.SetActive(false);
    }
}