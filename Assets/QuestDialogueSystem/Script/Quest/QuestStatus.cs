using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace QuestDialogueSystem
{
    public class QuestStatus
    {

        QuestData quest;
        public bool IsStarted {get; private set;}
        public bool IsCompleted {get; private set;}
        public bool IsFailed {get; private set;}

        public QuestStatus(QuestData quest)
        {
            this.quest = quest;
        }
        public void Start()
        {
            IsStarted = true;
            quest.onQuestStart?.Invoke();
            Locator.NotificationUI.PrintTitleMsg($"Quest \"{quest.questName}\" accepted");
        }
        public void Complete()
        {
            IsCompleted = true;
            Locator.NotificationUI.PrintTitleMsg($"Quest \"{quest.questName}\" completed");
            quest.onQuestComplete?.Invoke();
        }

        public void Fail()
        {
            IsFailed = true;
            quest.onQuestFail?.Invoke();
        }

        public void Cancel()
        {
            IsStarted = false;
        }

        public bool CanComplete()
        {

            List<ItemStack> requiredItems = quest.GetRequiredItemStacks();
            foreach(var stack in requiredItems)
            {
                if(!Locator.Inventory.TryRemove(stack, out int _))
                {
                    return false;
                }
            }
            return true;
        }
    }
}