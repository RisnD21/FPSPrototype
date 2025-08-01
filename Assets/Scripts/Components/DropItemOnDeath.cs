using Unity.VisualScripting;
using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;



public class DropItemOnDeath : MonoBehaviour
{
    [SerializeField] List<ItemStackData> items;
    public void Drop()
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        foreach (var item in items)
        {
            ItemStack stack = new(item.item, item.count);
            
            ItemManager.Instance.SpawnPickable(stack, (Vector2)pos + Random.insideUnitCircle);
        }
    }
}
