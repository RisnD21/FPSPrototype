using QuestDialogueSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUI : MonoBehaviour 
{
    public ItemData Item{get; private set;}

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI count;
    Color defaultColor;

    public bool IsEmpty => Item == null;
    void Awake()
    {
        defaultColor = icon.color;
        ClearSlot();
    }

    public void SetSlot(ItemData item)
    {
        int itemCount = Locator.Inventory.Count(item);
        if (item == null ||  itemCount == 0) 
        {
            ClearSlot();
            return;
        }

        Item = item;

        icon.sprite = Item.itemIcon;
        icon.color = defaultColor;
        icon.enabled = true;

        count.text = itemCount.ToString();

        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1,1,1,0); //transparent
        icon.enabled = false;
        count.text = "";
        Item = null;
        
        gameObject.SetActive(false);        
    }

    public void Refresh()
    {
        if(!IsEmpty) SetSlot(Item);
    }
}