using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestDialogueSystem;
using UnityEngine.EventSystems;


public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    InventorySlot slot;
    [SerializeField] Image icon;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI count;

    Color defaultColor;
    void Awake()
    {
        defaultColor = icon.color;
        slot = new();

        ClearSlot();
    }
    public void SetSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty) 
        {
            ClearSlot();
            return;
        }

        this.slot = slot;

        icon.sprite = this.slot.stack.Item.itemIcon;
        icon.color = defaultColor;
        icon.enabled = true;

        description.text = this.slot.stack.Item.description;
        count.text = this.slot.stack.Count.ToString();

        gameObject.SetActive(true);
    }

    void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1,1,1,0); //transparent
        icon.enabled = false;

        descriptionPanel.SetActive(false);
        count.text = "";

        if(slot == null) return;
        slot.stack = ItemStack.Empty;

        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot != null && !slot.IsEmpty)
        {
            descriptionPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot != null && !slot.IsEmpty)
        {
            
        }
    }

    public override string ToString()
    {
        if(slot == null || slot.IsEmpty) return "Empty Slot";
        return $"{slot.stack.Item.itemName} {slot.stack.Count}/{Mathf.Min(slot.stack.Max, slot.slotMaxStack)}";
    }
}
