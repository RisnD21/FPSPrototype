using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform slotPrefab;
    [SerializeField] int slotsCount = 24;
    List<SlotUI> slots;

    [SerializeField] GameObject itemMenu;
    [SerializeField] GameObject blocker;

    [SerializeField] Button singleButton;
    [SerializeField] Button upperButton;
    [SerializeField] Button lowerButton;

    [SerializeField] Button useItemButton;
    [SerializeField] Button dropItemButton;
    
    [SerializeField] Vector3 itemMenuOffset;

    [SerializeField] Damageable player;
    [SerializeField] TextMeshProUGUI healthPanel;

    public static event Action<InventorySlot> OnItemUse;
    InventorySlot currentSlot;
    public ItemData itemSelected;
    public void ItemSelected(ItemData item) => itemSelected = item;

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

    public void OpenItemMenu2(InventorySlot slot, Vector3 position)
    {
        //if only have single action, register it to the single button then enable it
        //otherwise register actions to upper/lower button, respectively, then enable them
    }

    public void OpenItemMenu(InventorySlot slot, Vector3 position)
    {
        currentSlot = slot;
        
        if (currentSlot.stack.Item.HasAction() && currentSlot.stack.Item.itemType != "Shield") 
        {
            useItemButton.onClick.RemoveAllListeners();
            useItemButton.onClick.AddListener(UseItem);
            useItemButton.gameObject.SetActive(true);
        } 
        else useItemButton.gameObject.SetActive(false);

        dropItemButton.onClick.RemoveAllListeners();

        if (currentSlot.stack.Item.CanDrop) 
            dropItemButton.onClick.AddListener(DestroyItem);
        else 
            dropItemButton.onClick.AddListener(ShowProhibitMsg);
        
        itemMenu.transform.position = position + itemMenuOffset;
        itemMenu.SetActive(true);
        blocker.SetActive(true);
    }

    void UseItem()
    {
        OnItemUse?.Invoke(currentSlot);
        CloseItemMenu();
    }

    public void UseItem(InventorySlot slot)
    {
        if(slot == null) return;
        OnItemUse?.Invoke(slot);
        CloseItemMenu();
    }

    void DestroyItem()
    {
        int toRemove = currentSlot.stack.Count;
        Locator.Inventory.TryRemoveFromSlot(currentSlot.stack, currentSlot, ref toRemove);
        CloseItemMenu();
    }

    void ShowProhibitMsg()
    {
        string msg = currentSlot.stack.Item.msgOnDrop ?? "Can't drop item";
        Locator.NotificationUI.PrintInventoryMsg(msg);
        CloseItemMenu();
    }

    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
        blocker.SetActive(false);
    }

    void Update()
    {
        healthPanel.text = player.Current + " / " + player.maxHealth.ToString();
    }
}
