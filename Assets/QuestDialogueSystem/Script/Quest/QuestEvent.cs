using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(fileName = "NewQuestEvent", menuName = "GameJam/Quest/QuestEvent")]
    public class QuestEvent : ScriptableEvent
    {
        public void Raised()
        {
            base.Raised();
        }
    }
}