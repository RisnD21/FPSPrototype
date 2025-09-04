using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace QuestDialogueSystem
{
    public class QuestUI : MonoBehaviour
    {
        Dictionary<QuestData, QuestBlock> questEntries = new();
        [SerializeField] RectTransform questInfoPanel;
        [SerializeField] QuestBlock questBlockPrefab;

        void OnEnable()
        {
            Locator.Inventory.OnItemAdd += CheckRequirement;
            Locator.Inventory.OnItemRemove += CheckRequirement;
        }

        void OnDisable()
        {
            Locator.Inventory.OnItemAdd -= CheckRequirement;
            Locator.Inventory.OnItemRemove -= CheckRequirement;
        }

        void CheckRequirement(ItemStack stack)
        {
            foreach(var entry in questEntries)
            {
                var quest = entry.Key;
                var block = entry.Value;

                if (quest.requireItem)
                {
                    var requiredStacks = quest.requiredItems;

                    foreach (var requireStack in requiredStacks)
                    {
                        var item = requireStack.item;
                        var required = requireStack.count;

                        if (!stack.Item.Equals(item)) continue;

                        var fulfillment = Locator.Inventory.Count(item) >= required;
                        block.SetRequirement(item.itemID, fulfillment);

                        block.UpdateProgression(item.itemID);
                        if (block.CanComplete) block.FulfillQuest();
                    }
                    StartCoroutine(UpdateUI(0.1f));
                } 
            }
        }

        public void AddQuest(QuestData quest) //called by questManager directly for simplicty
        {
            var block = Instantiate(questBlockPrefab, questInfoPanel);
            block.SetQuest(quest);
            
            StartCoroutine(UpdateUI(0.1f));

            questEntries[quest] = block;
        }

        public IEnumerator UpdateUI(float duration = 0)
        {
            if(duration > 0) yield return new WaitForSeconds(duration);
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(questInfoPanel);
        }

        public void CompleteQuest(QuestData quest) //same as above
        {
            questEntries.TryGetValue(quest, out var block);
            if(block != null)
            {
                block.PlayCompleteAnim();
                StartCoroutine(UpdateUI(2));
            }

            questEntries.Remove(quest);

        }
    }
}