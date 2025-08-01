using UnityEngine;
using System.Collections.Generic;
using System;

namespace QuestDialogueSystem
{
    public class Inventory : MonoBehaviour, IInventory
    {
        [SerializeField] int inventorySize = 30;
        InventoryModel model;

        public void Initialize()
        {
            model = new InventoryModel(inventorySize);

            model.OnItemAdd += stack => OnItemAdd?.Invoke(stack);
            model.OnItemRemove += stack => OnItemRemove?.Invoke(stack);
        }

        public InventoryModel GetModel() => model;

        public IReadOnlyList<InventorySlot> Slots => model.Slots;

        public int Count(string id) => model.Count(id);
        public int Count(ItemData item) => model.Count(item);

        public bool TryAdd(string id, int count, out int remainder)
            => model.TryAdd(id, count, out remainder);

        public bool TryAdd(ItemData item, int count, out int remainder)
            => model.TryAdd(item, count, out remainder);

        public bool TryAdd(ItemStack set, out int remainder)
            => model.TryAdd(set, out remainder);

        public bool TryRemove(string id, int count, out int remainder)
            => model.TryRemove(id, count, out remainder);

        public bool TryRemove(ItemData item, int count, out int remainder)
            => model.TryRemove(item, count, out remainder);

        public bool TryRemove(ItemStack set, out int remainder)
            => model.TryRemove(set, out remainder);

        public event Action<ItemStack> OnItemAdd;
        public event Action<ItemStack> OnItemRemove;
    }
}