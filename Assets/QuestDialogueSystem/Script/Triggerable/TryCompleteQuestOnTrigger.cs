using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/OnTrigger/Try Complete Quest")]
    public class TryCompleteQuestOnTrigger : ScriptableObject, ITriggerable
    {
        public QuestData quest;
        public void OnTrigger()
        {
            Locator.QuestManager.TryGetQuestStatus(quest, out var status);

            if(status.CanComplete())
            {
                Locator.QuestManager.CompleteQuest(quest);
            }
        }
    }
}