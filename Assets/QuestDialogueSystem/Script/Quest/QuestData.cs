using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace QuestDialogueSystem
{
    [CreateAssetMenu(menuName = "GameJam/Quest/Quest Data")]
    public class QuestData : ScriptableObject
    {
        public string id;
        public string questName;
        public Sprite icon;
        [TextArea(3,5)]
        public string description;
        public List<string> progressionTemplates;
        public string fulfillDescription;

        public bool requireItem;

        public List<ItemStackData> requiredItems;
        public UnityEvent onQuestStart;
        public UnityEvent onQuestCancel;
        public UnityEvent onQuestComplete;
        public UnityEvent onQuestFail;

        public List<ItemStack> GetRequiredItemStacks()
        {
            return requiredItems
                .Where(data => data.item != null)
                .Select(data => new ItemStack(data.item, data.count))
                .ToList();
        }
        
    }
}