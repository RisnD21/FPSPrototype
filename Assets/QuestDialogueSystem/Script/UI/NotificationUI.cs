using System.Collections;
using TMPro;
using UnityEngine;

namespace QuestDialogueSystem
{
    public class NotificationUI : MonoBehaviour, INotification
    {
        [SerializeField] TextMeshProUGUI titlePanel;
        [SerializeField] TextMeshProUGUI msgPanel;

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

        public void PrintTitleMsg(string msg)
        {
            if (titlePanel == null)
            {
                Debug.LogWarning("[NotificationUI] Notification Panel unassigned, disable messeage");
                return;
            }

            ShowMessage(titlePanel, msg);
            StartCoroutine(FadeOutText(titlePanel));
        }

        public void PrintInventoryMsg(string msg)
        {
            if (msgPanel == null)
            {
                Debug.LogWarning("[NotificationUI] Message Panel unassigned, disable messeage");
                return;
            }
            ShowMessage(msgPanel, msg);
            StartCoroutine(FadeOutText(msgPanel));
        }


        void OnItemAdd(ItemStack stack)
        {
            string msg = $"Collect {stack}";
            PrintInventoryMsg(msg);
        }

        void OnItemRemove(ItemStack stack)
        {
            string msg = $"Removed {stack}";
            PrintInventoryMsg(msg);
        }



        void ShowMessage(TextMeshProUGUI panel, string msg)
        {
            panel.gameObject.SetActive(true);
            
            panel.text = msg;
        }

        IEnumerator FadeOutText(TextMeshProUGUI panel)
        {
            yield return new WaitForSeconds(2);

            panel.gameObject.SetActive(false);
        }
    }
}