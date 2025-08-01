using UnityEngine;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/Quest/Quest Chain Entry")]
    public class QuestChainEntry : ScriptableObject
    {
        public QuestData quest;
        public ConversationScript befQuest;
        public ConversationScript onQuest;
        public ConversationScript aftQuest;   
    }
}