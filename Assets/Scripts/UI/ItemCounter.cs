using QuestDialogueSystem;
using TMPro;
using UnityEngine;

public class ItemCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI monitor;
    [SerializeField] ItemData item;
    int count;

    public void Initialize()
    {
        Locator.Inventory.Count(item);
        monitor.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        Locator.Inventory.OnItemAdd += Add;
        Locator.Inventory.OnItemRemove += Remove;
    }

    void OnDisable()
    {
        Locator.Inventory.OnItemAdd -= Add;
        Locator.Inventory.OnItemRemove -= Remove;
    }

    void Add(ItemStack stack)
    {
        if(!stack.Item.Equals(item)) return;
        count += stack.Count;
        UpdateMonitor();
    }

    void Remove(ItemStack stack)
    {
        if(!stack.Item.Equals(item)) return;
        count -= stack.Count;
        UpdateMonitor();
    }

    void UpdateMonitor()
    {
        monitor.text = count.ToString();
    }
}