using System.Collections.Generic;
using QuestDialogueSystem;
using UnityEngine;

public class QuestNarrator : MonoBehaviour
{
    [SerializeField] List<QuestChainEntry> questChainEntries = new();

    QuestChainEntry currentQuestEntry;
    ConversationScript currentScript;
    int progressionIndex;
    bool isDebugMode = false;

    public void Interact()
    {
        if(isDebugMode) Debug.Log("Interacting with " + gameObject.name);

        SelectEntry();
        SelectScript();

        Locator.DialogueRunner.StartConversation(currentScript);
    }

    void SelectEntry()
    {
        if (progressionIndex >= questChainEntries.Count) return;
        currentQuestEntry = questChainEntries[progressionIndex];
    }

    void SelectScript()
    {
        if(isDebugMode) Debug.Log("Showing Script");
        QuestData quest = currentQuestEntry.quest;

        if(quest == null) Debug.LogError("Quest is null");
        if(Locator.QuestManager == null) Debug.LogError("QM is null");

        if(Locator.QuestManager.TryGetQuestStatus(quest, out var questStatus))
        {
            if (questStatus.IsCompleted)
            {
                currentScript = currentQuestEntry.aftQuest;
                progressionIndex++;
                return;
            } else if (questStatus.IsStarted)
            {
                currentScript = currentQuestEntry.onQuest;
                return;
            }
        }

        currentScript = currentQuestEntry.befQuest;
    }
}