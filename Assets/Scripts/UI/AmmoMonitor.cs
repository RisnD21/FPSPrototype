using QuestDialogueSystem;
using TMPro;
using UnityEngine;

public class AmmoMonitor : MonoBehaviour
{
    TextMeshProUGUI monitor;
    ItemData currentAmmoType;
    int current;

    void Awake()
    {
        monitor = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Locator.Inventory.OnItemAdd += UpdateAmmoCount;
    }

    void OnDisable()
    {
        Locator.Inventory.OnItemAdd -= UpdateAmmoCount;
    }

    void UpdateAmmoCount(ItemStack stack)
    {
        if(stack.Item.Equals(currentAmmoType))
        {
            SyncAmmo(currentAmmoType, current, Locator.Inventory.Count(currentAmmoType));
        }
    }

    public void SyncAmmo(ItemData currentType, int current, int reserved)
    {
        currentAmmoType = currentType;
        this.current = current;
        monitor.text = $"{this.current}/{reserved}";
    }
}