using NUnit.Framework;
using UnityEngine;

namespace QuestDialogueSystem
{
    [TestFixture]
    public class InventorySlotTests
    {
        public ItemData CreateItem(string id, int maxStack = 99)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            item.itemID = id;
            item.itemName = "TestItem_" + id;
            item.maxStack = maxStack;
            item.hideFlags = HideFlags.HideAndDontSave;
            return item;            
        }

        [Test]
        public void AddStack_WhenSlotIsEmpty_AddsCorrectAmount()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 10);

            
            Assert.IsTrue(slot.IsEmpty, "Slot should be empty befor add");
            Assert.IsTrue(slot.CanStackWith(item), "Slot should be empty.");
            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder), "TryAtomicAdd failed");
            Assert.AreEqual(0, remainder, "Remainder should be 0 after add");
            Assert.IsTrue(slot.HasItem(item), "Comparing: " + item.itemID + ", " + slot.stack.Item);
            Assert.IsFalse(slot.IsEmpty, "Slot should not be empty after add");
            Assert.IsFalse(slot.IsFull, "Slot should not be full after add (only 10)");
            Assert.AreEqual(10, slot.stack.Count, "Slot count should be 10 after add");
        }

        [Test]
        public void AddStack_WhenSlotIsPartiallyFull_AddsCorrectAmount()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);            

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder));
            Assert.IsTrue(slot.TryAtomicAdd(stack, out int newRemainder));
            newRemainder += remainder;
            Assert.AreEqual(0, newRemainder, "Remainder should be 0 after add");
            Assert.AreEqual(80, slot.stack.Count, "Slot count should be 10 after add");
        } 

        [Test]
        public void AddStack_WhenSlotIsPartiallyFull_StacksUpToLimit()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);  
            ItemStack stack2 = new(item, 100);            

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder));
            Assert.AreEqual(0, remainder, "Remainder should be 0 after add");

            Assert.IsTrue(slot.TryAtomicAdd(stack2, out int newRemainder));
            Assert.AreEqual(41, newRemainder, "Remainder should be 41 after add");
            Assert.IsTrue(slot.IsFull, "Full slot should not accept stack");
            Assert.IsFalse(slot.CanStackWith(item), "Full slot should not accept stack");
        }       
             

        [Test]
        public void AddStack_WhenItemMismatch_Fails()
        {
            var item = CreateItem("Apple");
            var item2 = CreateItem("Banana");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);  
            ItemStack stack2 = new(item2, 40);  

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder), "Fill slot with apples");
            Assert.IsFalse(slot.CanStackWith(stack2.Item), "Items mismatch");
            Assert.IsFalse(slot.TryAtomicAdd(stack2, out int newRemainder), "Different items can not be stacked");

            Assert.IsFalse(slot.TryAtomicRemove(stack2, out newRemainder), "Different items can not be removed");
        }

        [Test]
        public void RemoveStack_WhenSlotIsEmpty()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);  

            Assert.IsFalse(slot.TryAtomicRemove(stack, out int remainder));
            Assert.AreEqual(40, remainder,"remainder is " + remainder);
            Assert.IsTrue(slot.IsEmpty, "Slot should be empty");
        } 

        [Test]
        public void RemoveStack_WhenSlotIsPartiallyFull_RemoveSome()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);  

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder));
            Assert.IsTrue(slot.TryAtomicRemove(new ItemStack(item, 30), out remainder));
            Assert.IsTrue(slot.TryAtomicRemove(new ItemStack(item, 10), out remainder));
            Assert.IsTrue(slot.IsEmpty, "Slot should be empty aft removing 40 apples");
            Assert.IsFalse(slot.HasItem(item), "Slot should not contain apple aft removal");
        }       

        [Test]
        public void RemoveStack_WhenSlotIsPartiallyFull_OverRemove()
        {
            var item = CreateItem("Apple");
            InventorySlot slot = new();
            ItemStack stack = new(item, 40);  

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder));
            Assert.IsTrue(slot.TryAtomicRemove(new ItemStack(item, 50), out remainder));
            Assert.AreEqual(10, remainder);
            Assert.IsTrue(slot.TryAtomicAdd(stack, out remainder));
            Assert.IsTrue(slot.TryAtomicRemove(new ItemStack(item, 500), out remainder));
            Assert.AreEqual(460, remainder);
        }     

        [Test] 
        public void ModifyStack_CustomMaxStack()
        {
            var item = CreateItem("Apple", 20);
            InventorySlot slot = new();
            ItemStack stack = new(item, 20); 

            Assert.AreEqual(20, item.maxStack, "Max stack is " + item.maxStack);

            Assert.IsTrue(slot.TryAtomicAdd(stack, out int remainder), "Can't Add");
            Assert.AreEqual(0, remainder);
            Assert.IsFalse(slot.TryAtomicAdd(stack, out remainder), "Slot can add");
            Assert.AreEqual(20, slot.stack.Count, "Has " + slot.stack.Count + " Apples");
            Assert.AreEqual(20, remainder);
            Assert.IsTrue(slot.HasItem(item),"Slot doesn't have " + item.itemName);
            Assert.IsTrue(slot.TryAtomicRemove(new ItemStack(item, 50), out remainder), "Removal Invalid");
            Assert.IsTrue(slot.IsEmpty, "Has " + slot.stack.Count + " Apples");
            Assert.AreEqual(30, remainder);
        }
    }

}