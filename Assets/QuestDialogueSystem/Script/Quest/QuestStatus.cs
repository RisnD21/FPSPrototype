using System.Collections.Generic;

namespace QuestDialogueSystem
{
    public class QuestStatus
    {

        QuestData quest;
        QuestUI questUI;
        public bool IsStarted {get; private set;}
        public bool IsCompleted {get; private set;}
        public bool IsFailed {get; private set;}

        public QuestStatus(QuestData quest, QuestUI ui)
        {
            this.quest = quest;
            questUI = ui;
        }
        public void Start()
        {
            IsStarted = true;
            quest.onQuestStart?.Invoke();
            Locator.NotificationUI.PrintTitleMsg($"{quest.questName}");
            questUI.AddQuest(quest);
        }
        public void Complete()
        {
            IsCompleted = true;
            Locator.NotificationUI.PrintTitleMsg($"{quest.questName} <b>完成</b>");
            quest.onQuestComplete?.Invoke();
            questUI.CompleteQuest(quest);
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