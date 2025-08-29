
using UnityEngine;
using System.Collections.Generic;

namespace QuestDialogueSystem
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] QuestUI questUI;
        public List<QuestData> quests = new();

        Dictionary<QuestData, QuestStatus> statuses = new();

        public bool ContainQuest(QuestData quest) => statuses.ContainsKey(quest);

        public bool TryGetQuestStatus(QuestData quest, out QuestStatus status)
        {
            if (statuses.TryGetValue(quest, out var questStatus))
            {
                status = questStatus;
                return true;
            } else
            {
                status = null;
                return false;
            }
        }

        public bool TryGetQuest(string id, out QuestData quest)
        {
            foreach (var entry in quests)
            {
                if (entry.id == id)
                {
                    quest = entry;
                    return true;
                }
            } 

            quest = null;
            return false;
        }

        public void StartQuest(QuestData quest)
        {
            if (!ContainQuest(quest))
            {
                AddQuestWithoutStart(quest);
            }
            
            statuses[quest].Start();
        }

        public void AddQuestWithoutStart(QuestData quest)
        {
            if (ContainQuest(quest)) 
            {
                Debug.LogWarning("[QuestManager] Quest already exist, abort adding quest");
                return;
            }

            quests.Add(quest);
            statuses[quest] = new QuestStatus(quest, questUI);
        }

        public void CancelQuest(QuestData quest)
        {
            if (!ContainQuest(quest))
            {
                Debug.LogWarning("[QuestManager] Quest not found, abort cancelling");
                return;
            }

            statuses[quest].Cancel();
        }

        public void CompleteQuest(QuestData quest)
        {
            if(!ContainQuest(quest))
            {
                Debug.LogWarning("[QuestManager] Quest not found, abort completing");
                return;
            }
            statuses[quest].Complete();            
        }

        public void FailQuest(QuestData quest)
        {
            if(!ContainQuest(quest))
            {
                Debug.LogWarning("[QuestManager] Quest not found, abort failing");
                return;
            }

            statuses[quest].Fail();            
        }

        public void RemoveQuest(QuestData quest)
        {
            if (!ContainQuest(quest)) return;

            quests.Remove(quest);
            statuses.Remove(quest);
        }

        public void RemoveQuest(string id)
        {
            if (TryGetQuest(id, out var quest))
            {
                RemoveQuest(quest);
                return;
            }
        }

        public void ClearAllQuestList()
        {
            quests.Clear();
            statuses.Clear();
        }
    }
}