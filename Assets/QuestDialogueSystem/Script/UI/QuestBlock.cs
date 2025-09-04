using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

namespace QuestDialogueSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public class QuestBlock : MonoBehaviour
    {
        [Header("Title / Description")]
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI description;

        [Header("Progress UI")]
        [SerializeField] RectTransform progressionParent; 
        [SerializeField] TextMeshProUGUI progressionPrefab;
        Dictionary<string, TextMeshProUGUI> progressionEntryIndex = new();
        Dictionary<string, string> progressionTemplateIndex = new();

        [Header("Fulfillment")]
        [SerializeField] TextMeshProUGUI fulfillment;


        CanvasGroup blockCanvasGroup;
        QuestData quest;
        bool isDebugMode = false;

        Dictionary<string, int> requirementIndex = new();
        Dictionary<string, bool> requirements = new();
        public void SetRequirement(string id, bool fulfill) => requirements[id] = fulfill;

        public bool CanComplete 
        {
            get 
            {
                foreach (var state in requirements) if (!state.Value) return false;
                return true;
            }
        }

        void Awake()
        {
            blockCanvasGroup = GetComponent<CanvasGroup>();
            ClearUI();
        }
        
        void OnDestroy() => ClearTween();

        void ClearTween()
        {
            DOTween.Kill(this, complete: false);
            if (blockCanvasGroup != null) DOTween.Kill(blockCanvasGroup, complete: false);
        }


        void ClearUI()
        {
            ClearTween();

            DestroyProgressionEntries();
            progressionEntryIndex.Clear();
            progressionTemplateIndex.Clear();
            requirementIndex.Clear();
            requirements.Clear();

            if (title)
            {
                title.text = "";
                title.gameObject.SetActive(false);
                title.transform.parent.gameObject.SetActive(false);
                
            }
            if (description)
            {
                description.text = "";
                description.gameObject.SetActive(false);
                
            }
            if (fulfillment)
            {
                fulfillment.text = "";
                fulfillment.gameObject.SetActive(false);
                
            }
        }

        void DestroyProgressionEntries()
        {
            GameObject[] progressionEntries = progressionEntryIndex.Select(e => e.Value.gameObject).ToArray();
            for (int i = progressionEntries.Length - 1; i > -1; i --)
            {
                if(progressionEntries[i] != null) Destroy(progressionEntries[i]);
            }
        }

        public void SetQuest(QuestData quest)
        {
            this.quest = quest;
            SetTitle();
            SetDescription();

            if(quest.requireItem)
            {
                SetupRequirementIndex();   
                SetupProgressionEntries();
            }

            PlayQuestIntroAnim();
        }

        void SetTitle()
        {
            title.text = quest.questName;
        }

        void SetDescription()
        {
            description.text = quest.description;
        }

        void SetupProgressionEntries()
        {
            var toSearch = ItemDatabase.itemList.ToArray();

            foreach (var entry in quest.progressionTemplates)
            {
                bool misMatch = true;

                for(int i = toSearch.Length - 1; i > -1; i--)
                {
                    var itemID = toSearch[i];

                    if(!entry.Contains("{" + itemID + "}")) continue;
                    
                    misMatch = false;
                    progressionTemplateIndex[itemID] = entry;
                    if(isDebugMode)Debug.Log($"Binding Templates {itemID}/{entry}");

                    UpdateProgression(itemID);
                }

                if(misMatch) 
                {
                    Debug.LogError($"[QuestBlock] Unrecognize itemID exists in {entry}");
                    return;
                }
            }                
        }

        void SetupRequirementIndex()
        {
            List<ItemStackData> requireItems = quest.requiredItems;
            foreach (var stack in requireItems)
            {
                string itemID = stack.item.itemID;

                progressionEntryIndex[itemID] = Instantiate(progressionPrefab, progressionParent);
                requirementIndex[itemID] = stack.count;
                if(isDebugMode)Debug.Log($"progression index: {itemID}/{stack.count}");
            }
        }

        void PlayQuestIntroAnim()
        {
            if (blockCanvasGroup == null) return;
            blockCanvasGroup.DOKill();

            title.transform.parent.gameObject.SetActive(true);
            title.gameObject.SetActive(true);
            description.gameObject.SetActive(true);
            fulfillment.gameObject.SetActive(true);

            float duration = 1f;
            blockCanvasGroup.alpha = 0f;
            blockCanvasGroup.DOFade(1f, duration).SetId(this);
        }

        public void UpdateProgression(string id)
        {
            if(isDebugMode)Debug.Log($"Updating progress {id}");
            if(!progressionEntryIndex.TryGetValue(id, out var progressionEntry))
            {
                Debug.LogError($"[QuestBlock] Block desync on entry {quest.questName}");
                return;
            }

            progressionTemplateIndex.TryGetValue(id, out var entry);
            progressionEntry.text = RenderProgressionEntry(entry, id);   
        }

        string RenderProgressionEntry(string target, string itemID)
        {
            ItemDatabase.TryGetItemData(itemID, out var item);
            var itemName = item.itemName;
            var currentHave = Locator.Inventory.Count(itemID);
            requirementIndex.TryGetValue(itemID, out var amountRequired);

            // 依進度決定顏色（0% 紅 → 100% 綠）
            Color color = Color.Lerp(Color.red, Color.green, Mathf.Clamp01((float)currentHave / amountRequired));
            string progressHex = ColorUtility.ToHtmlStringRGB(color);

            string itemHex = "FFD700"; // 金色 or else

            //ex. "收集 <b><color=#{itemColor}>{itemName}</color></b>（<color=#{progressHex}>{current}</color>/{required}）"
            
            var vars = new Dictionary<string, object>
            {
                ["current"]       = currentHave,
                ["required"]      = amountRequired,
                ["progressHex"]   = progressHex,
                ["itemColor"]     = itemHex
            };

            vars[itemID] = itemName;
            vars[itemID] = "";

            return SimpleTemplate.Format(target, vars);
        }

        public void FulfillQuest()
        {
            if (isDebugMode) Debug.Log("requirement met");
            fulfillment.text = quest.fulfillDescription;
        }   

        public void PlayCompleteAnim()
        {
            if (title != null) title.text = $"<s>{title.text}</s>";

            if (blockCanvasGroup != null)
            {
                blockCanvasGroup.DOKill(); 
                blockCanvasGroup.DOFade(0f, 2f)
                    .OnComplete(ClearUI)
                    .SetId(this);
            }
            else ClearUI(); 
        }
    }
}