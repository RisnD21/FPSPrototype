
using QuestDialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "GameJam/Item/ItemAction/Heal")]
public class HealHealth : ItemAction
{
    [SerializeField] int amount;
    [SerializeField] float duration;
    [SerializeField] bool canBeDisrupted;

    public override bool TryUse(UseContext useContext, InventorySlot slot)
    {
        ItemData currentItem = slot.stack.Item;
        string itemID = currentItem.itemID;
        int toConsumed = 1;
        ItemStack stack = new(currentItem, toConsumed);

        if(!useContext.inventory.TryRemoveFromSlot(stack, slot, ref toConsumed)) return false;
        
        HealingEffect effect = new(itemID, amount, duration, EffectMode.reset, canBeDisrupted);

        RegenOverTime regenerator = useContext.user.GetComponent<RegenOverTime>();
        regenerator.AddEffect(effect);
        return true;
    }

    public override bool TryUse(UseContext useContext, ItemData item)
    {
        ItemData currentItem = item;
        string itemID = currentItem.itemID;
        int toConsumed = 1;
        ItemStack stack = new(currentItem, toConsumed);

        if(!useContext.inventory.TryRemove(stack, out _)) return false;
        
        HealingEffect effect = new(itemID, amount, duration, EffectMode.reset, canBeDisrupted);

        RegenOverTime regenerator = useContext.user.GetComponent<RegenOverTime>();
        regenerator.AddEffect(effect);

        VFXManager.Instance.SpawnHealEffect(regenerator.gameObject.transform);
        
        return true;
    }
}