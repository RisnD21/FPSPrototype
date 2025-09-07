using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestDialogueSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TooltipHoverHandler))]
public class SlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public InventorySlot Slot{get; private set;}
    [SerializeField] Image icon;
    [SerializeField] Image highlight;
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
        GetComponent<TooltipHoverHandler>().SetTooltip(slot.stack.Item.description);

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

    void OnEnable()
    {
        SetImageAlpha(highlight, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Slot == null || Slot.IsEmpty) return;
        inventoryUI.OpenItemMenu(Slot, eventData.position);
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryUI.ItemSelected(Slot.stack.Item);
        SetImageAlpha(highlight, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.ItemSelected(null);
        SetImageAlpha(highlight, 0);
    }

    void SetImageAlpha(Image img, float a)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, a);
    }

    public override string ToString()
    {
        if(Slot == null || Slot.IsEmpty) return "Empty Slot";
        return $"{Slot.stack.Item.itemName} {Slot.stack.Count}/{Mathf.Min(Slot.stack.Max, Slot.slotMaxStack)}";
    }
}
