using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace QuestDialogueSystem
{
    public class MsgEntry
    {
        public string text;

        float enterTimestamp;
        float displayDuration = 4f;

        public bool MustDisplay => Time.time - enterTimestamp > displayDuration;

        public MsgEntry(string text)
        {
            this.text = text;
            enterTimestamp = Time.time;
        }
    }

    public class MsgArranger : MonoBehaviour, INotification
    {
        const int EntriesAmount = 7;

        // --- Queues ---
        Queue<MsgEntry> pendingQueue = new();
        Queue<MsgView> displayingQueue = new();

        // --- Display control ---
        [SerializeField] MsgView[] views = new MsgView[EntriesAmount];
        [SerializeField] TextMeshProUGUI titlePanel;
        [SerializeField] TitleController titleController;

        float msgDisplayInterval = 0.1f;
        float msgDisplayCooldown;
        int currentIndex;

        // --- Unity lifecycle ---
        void OnEnable()
        {
            Locator.Inventory.OnItemAdd += OnItemAdd;
            Locator.Inventory.OnItemRemove += OnItemRemove;
        }

        void OnDisable()
        {
            Locator.Inventory.OnItemAdd -= OnItemAdd;
            Locator.Inventory.OnItemRemove -= OnItemRemove;
        }

        // --- Inventory notifications ---
        void OnItemAdd(ItemStack stack)
        {
            PrintInventoryMsg($"獲得 {stack}");
        }

        void OnItemRemove(ItemStack stack)
        {
            PrintInventoryMsg($"移除 {stack}");
        }

        // --- Public API ---
        public void PrintTitleMsg(string text)
        {
            if(titleController != null) titleController.PrintText(text);
            
        }

        public void PrintInventoryMsg(string text)
        {
            pendingQueue.Enqueue(new MsgEntry(text));
        }

        // --- Internal queue handling ---
        void DequeuePendingQueue()
        {
            EnqueueDisplayingQueue(pendingQueue.Dequeue());
        }

        void EnqueueDisplayingQueue(MsgEntry msg)
        {
            MsgView newView = views[currentIndex];
            newView.SetText(msg.text);

            currentIndex = (currentIndex + 1) % views.Length;

            if (displayingQueue.Count > 4)
                AccelerateDisplay();
            else if (displayingQueue.Count > 0)
                MoveAllViewsDownward(0.2f);

            displayingQueue.Enqueue(newView);
            newView.FadeIn(0.2f);
        }

        void MoveAllViewsDownward(float duration)
        {
            foreach (var entry in displayingQueue)
                entry.MoveDownward(duration);
        }

        public void DequeueDisplayingQueue()
        {
            var view = displayingQueue.Dequeue();
            view.Reset();
        }

        public void AccelerateDisplay()
        {
            MoveAllViewsDownward(0.05f);
            displayingQueue.Peek().FadeOut(0.05f);
        }

        // --- Update loop ---
        void Update()
        {
            CheckPendingQueue();
            CheckDisplayingQueue();
        }

        void CheckPendingQueue()
        {
            if (pendingQueue.Count == 0) return;

            if (pendingQueue.Peek().MustDisplay) DequeuePendingQueue();
            else if (msgDisplayCooldown <= 0)
            {
                DequeuePendingQueue();
                msgDisplayCooldown = msgDisplayInterval;
            }
            else msgDisplayCooldown -= Time.deltaTime;
        }

        void CheckDisplayingQueue()
        {
            if (displayingQueue.Count == 0) return;

            while (displayingQueue.Peek().DoneDisplaying)
            {
                DequeueDisplayingQueue();
                if (displayingQueue.Count == 0) return;
            }

            foreach (var view in displayingQueue)
            {
                if (view == null) continue;

                if (view.ShouldFade && !view.IsFadingOut)
                    view.FadeOut(2f);
                else
                    view.displayedTime += Time.deltaTime;
            }
        }
    }
}
