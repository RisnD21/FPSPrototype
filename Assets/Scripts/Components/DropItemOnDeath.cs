using Unity.VisualScripting;
using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;



public class DropItemOnDeath : MonoBehaviour
{
    [SerializeField] bool dropInventoryItemInstead;
    [SerializeField] List<ItemStackData> items;
    List<ItemStack> stacks;

    InventoryModel inventory;


    void Awake()
    {
        stacks = new();
    }
    void Start()
    {
        inventory = GetComponent<Inventory>().GetModel();
    }

    void PrepareStacks()
    {
        stacks.Clear();

        if (dropInventoryItemInstead)
        {
            stacks = inventory.RetriveAllStacks();
        }else
        {
            foreach (var item in items)
            {
                ItemStack stack = new(item.item, item.count);
                stacks.Add(stack);
            }
        }
    }

    public void Drop()
    {
        Vector3 pos = transform.position;
        pos.z = 0;

        PrepareStacks();

        foreach(var stack in stacks)
        {
            ItemManager.Instance.SpawnPickable(stack, (Vector2)pos + Random.insideUnitCircle);
        }
    }
}
