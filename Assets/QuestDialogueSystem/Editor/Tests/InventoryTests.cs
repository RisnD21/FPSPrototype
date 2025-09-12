using NUnit.Framework;
using UnityEngine;

namespace QuestDialogueSystem
{
    [TestFixture]
    public class InventoryTests
    {
        public ItemData CreateItem(string id, int maxStack = 999)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            item.itemID = id;
            item.itemName = "TestItem_" + id;
            item.maxStack = maxStack;
            item.hideFlags = HideFlags.HideAndDontSave;
            return item;            
        }

        [Test]
        public void CountStack()
        {
            ItemData apple = CreateItem("Apple", 3);
            ItemData banana = CreateItem("Banana", 5);
            ItemData lemon = CreateItem("Lemon", 7);
            
            InventoryModel inventory = new(5);
            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 3), out int remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(banana, 1), out remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(lemon, 2), out remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 2), out remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(banana, 1), out remainder));

            Assert.AreEqual(inventory.Count(apple), 5);
            Assert.AreEqual(inventory.Count(banana), 2);
            Assert.AreEqual(inventory.Count(lemon), 2);
        }

        [Test]
        public void CheckSpace()
        {
            ItemData apple = CreateItem("Apple", 3);
            ItemData banana = CreateItem("Banana", 5);
            ItemData lemon = CreateItem("Lemon", 7);
            
            InventoryModel inventory = new(5);
            inventory.TryAdd(new ItemStack(apple, 3), out int remainder);
            inventory.TryAdd(new ItemStack(banana, 1), out remainder);
            inventory.TryAdd(new ItemStack(lemon, 2), out remainder);
            inventory.TryAdd(new ItemStack(apple, 2), out remainder);
            inventory.TryAdd(new ItemStack(banana, 1), out remainder);

            Assert.AreEqual(inventory.SpaceLeftFor(apple), 4);
            Assert.AreEqual(inventory.SpaceLeftFor(banana), 8);
            Assert.AreEqual(inventory.SpaceLeftFor(lemon), 12);
        }


        [Test]
        public void AddStack_StillHaveSpace()
        {
            ItemData item = CreateItem("Apple");
            InventoryModel inventory = new(5);

            Assert.AreEqual(inventory.SpaceLeftFor(item), 5*99, "Space for apple, cal error");

            ItemStack stack = new(item, 100);
            Assert.IsTrue(inventory.TryAdd(stack, out int remainder));
            Assert.AreEqual(remainder, 0);
            Assert.AreEqual(inventory.SpaceLeftFor(item), 395, "Space mismatch");

            ItemData item2 = CreateItem("Banana", 10);
            Assert.AreEqual(inventory.SpaceLeftFor(item2), 30, "Space for banana bef apple, cal error");
            stack = new(item2, 30);
            Assert.IsTrue(inventory.TryAdd(stack, out _));

            stack = new(item, 98);
            Assert.IsTrue(inventory.TryAdd(stack, out _));
            Assert.AreEqual(inventory.SpaceLeftFor(item2), 0, "Space for banana aft apple, cal error");


        } 

        [Test] 
        public void AddStack_NotEnoughSpace()
        {
            ItemData apple = CreateItem("Apple",10);
            InventoryModel inventory = new(5);

            ItemStack stack  = new(apple, 45);
            Assert.IsTrue(inventory.TryAdd(stack, out int remainder));
            Assert.AreEqual(remainder, 0);

            Assert.AreEqual(inventory.SpaceLeftFor(apple), 5);
            Assert.IsFalse(inventory.TryAdd(stack, out remainder));
            Assert.AreEqual(remainder, 45);

            Assert.IsTrue(inventory.TryForceAdd(stack, out remainder));
            Assert.AreEqual(remainder, 40);
            Assert.AreEqual(inventory.SpaceLeftFor(apple), 0);
        }

        [Test]
        public void RemoveStack_HasEnoughItem()
        {
            ItemData apple = CreateItem("Apple",10);
            InventoryModel inventory = new(5);

            ItemStack stack  = new(apple, 45);
            inventory.TryAdd(stack, out int remainder);
            
            Assert.IsTrue(inventory.TryRemove(new ItemStack(apple, 35), out remainder));
            Assert.AreEqual(inventory.Count(apple), 10);
        }

        [Test]
        public void RemoveStack_DontHaveEnoughItem()
        {
            ItemData apple = CreateItem("Apple",10);
            InventoryModel inventory = new(5);

            ItemStack stack  = new(apple, 45);
            inventory.TryAdd(stack, out int remainder);
            
            Assert.IsFalse(inventory.TryRemove(new ItemStack(apple, 100), out remainder),"100 removed");
            Assert.AreEqual(inventory.Count(apple), 45);
            Assert.AreEqual(remainder, 100);

            Assert.IsTrue(inventory.TryForceRemove(new ItemStack(apple, 80), out remainder), "non removed");
            Assert.AreEqual(inventory.Count(apple), 0);
            Assert.AreEqual(remainder, 35);
        }

        //special edge case
        [Test]
        public void RemoveStack_NonExistItem()
        {
            ItemData apple = CreateItem("Apple",10);
            ItemData banana = CreateItem("Banana",5);
            InventoryModel inventory = new(5);

            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 40), out int remainder));
            Assert.AreEqual(inventory.Count(banana), 0);

            Assert.IsFalse(inventory.TryRemove(new ItemStack(banana, 10), out remainder));
            Assert.AreEqual(remainder, 10);
        }

        [Test]
        public void AddStack_DifferentItemWhenFull()
        {
            ItemData apple = CreateItem("Apple",10);
            ItemData banana = CreateItem("Banana",5);
            ItemData orange = CreateItem("Orange",5);
            InventoryModel inventory = new(5);

            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 10), out int remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(banana, 9), out  remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(orange, 4), out  remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 4), out  remainder));
            
            Assert.IsTrue(inventory.TryAdd(new ItemStack(apple, 2), out  remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(orange, 1), out  remainder));
            Assert.IsFalse(inventory.TryAdd(new ItemStack(banana, 2), out  remainder));
            Assert.IsTrue(inventory.TryAdd(new ItemStack(banana, 1), out  remainder));
        }

    }

}