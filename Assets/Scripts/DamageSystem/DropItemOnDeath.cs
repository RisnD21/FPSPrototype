using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;

public class DropItemOnDeath : MonoBehaviour, IHealthListener
{
    [SerializeField] bool dropInventoryItemInstead;
    [SerializeField] List<ItemStackData> items;

    List<ItemStack> _cached;

    InventoryModel inventory;
    bool dropped;
    

    void Awake()
    {
        _cached = new();
        
    }
    void Start()
    {
        var inv = GetComponent<Inventory>();
        inventory = inv != null? inv.GetModel() : null;
    }

    public void OnDeath()
    {
        if (dropped) return;
        dropped = true;
        Drop();
    }

    public void OnHealthChanged(int current, int max) { }
    public void OnDamaged(int amount) { }
    public void OnHealed(int amount) { }

    void Drop()
    {
        Vector3 pos = transform.position; pos.z = 0;
        
        PrepareStacks();

        foreach(var stack in _cached)
        {
            ItemManager.Instance.SpawnPickable(stack, (Vector2)pos + Random.insideUnitCircle);
        }
    }

    void PrepareStacks()
    {
        _cached.Clear();

        if (dropInventoryItemInstead && inventory != null)
        {
            var all = inventory.RetriveAllStacks();
            foreach (var s in all) _cached.Add(new ItemStack(s.Item, s.Count));
            inventory.ClearInventory();
        }else
        {
            foreach (var s in items)
            {
                _cached.Add(new ItemStack(s.item, s.count));
            }
        }
    }
}
