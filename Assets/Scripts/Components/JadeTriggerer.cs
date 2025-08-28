using QuestDialogueSystem;
using UnityEngine;

[RequireComponent(typeof(IInventory))]
public class JadeTriggerer : MonoBehaviour
{
    [SerializeField] QuestData quest;
    IInventory inventory;
    void Awake()
    {
        inventory = GetComponent<IInventory>();
    }

    void OnEnable()
    {
        inventory.OnItemAdd += IfAddJade;
    }

    void OnDisable()
    {
        inventory.OnItemAdd -= IfAddJade;
    }

    void IfAddJade(ItemStack stack)
    {
        if(stack.Item.itemType != "Jade") return;
        TriggerQuest();
    }

    void TriggerQuest()
    {
        Locator.QuestManager.StartQuest(quest);
    }
}