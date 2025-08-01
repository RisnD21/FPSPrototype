using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/OnTrigger/Start Quest")]
    public class StartQuestOnTrigger : ScriptableObject, ITriggerable
    {
        public QuestData quest;
        public void OnTrigger()
        {
            Locator.QuestManager.StartQuest(quest);
        }
    }
}