using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestDialogueSystem;
using UnityEngine.EventSystems;
using System;


public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public InventorySlot Slot{get; private set;}
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI count;
    Color defaultColor;
    InventoryUI inventoryUI;

    public void InitializeSlot(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;

        defaultColor = icon.color;
        Slot = new();

        ClearSlot();
    }

    public void SetSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty) 
        {
            ClearSlot();
            return;
        }

        Slot = slot;

        icon.sprite = Slot.stack.Item.itemIcon;
        icon.color = defaultColor;
        icon.enabled = true;

        count.text = Slot.stack.Count.ToString();

        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1,1,1,0); //transparent
        icon.enabled = false;

        if(Slot == null) return;
        Slot.stack = ItemStack.Empty;

        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Slot == null || Slot.IsEmpty) return;
        inventoryUI.ShowDescriptionPanel(Slot, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.HideDescriptionPanel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Slot == null || Slot.IsEmpty) return;
        inventoryUI.OpenItemMenu(Slot, eventData.position);
    }

    public override string ToString()
    {
        if(Slot == null || Slot.IsEmpty) return "Empty Slot";
        return $"{Slot.stack.Item.itemName} {Slot.stack.Count}/{Mathf.Min(Slot.stack.Max, Slot.slotMaxStack)}";
    }
}
