using System.Collections.Generic;
using QuestDialogueSystem;
using UnityEngine;

[RequireComponent(typeof(IInventory))]
public class ShieldTriggerer : MonoBehaviour
{
    IInventory inventory;
    [SerializeField] GameObject shield; //Assign shield object

    UseContext ctx;

    void Awake()
    {
        AssembleUseContext();
        inventory = GetComponent<IInventory>();
    }

    void AssembleUseContext()
    {
        ctx = new(shield);
    }

    void OnEnable()
    {
        inventory.OnItemAdd += IfAddShield;
        inventory.OnItemRemove += IfRemoveShield;
    }

    void OnDisable()
    {
        inventory.OnItemAdd -= IfAddShield;
        inventory.OnItemRemove -= IfRemoveShield;
    }

    //useShield automatically
    void IfAddShield(ItemStack stack)
    {
        if (stack.Item.itemType != "Shield") return;
        Debug.Log($"{gameObject.name} Adding Shield");
        shield.SetActive(true);
        stack.Item.itemAction.TryUse(ctx, stack.Item);
    }

    //嘗試使用物品欄中的每一個 Shield
    void IfRemoveShield(ItemStack stack)
    {
        if(stack.Item.itemType != "Shield") return;
        bool hasShield = false;

        ItemDatabase.TryGetItemsByType("Shield", out List<ItemData> shieldVariety);
        foreach (var item in shieldVariety) 
        {
            if(inventory.Count(item) > 0)
            {
                item.itemAction.TryUse(ctx, item);
                hasShield = true;
            }
        }

        if(!hasShield) shield.SetActive(false);
    }
}
