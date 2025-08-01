using UnityEngine;

namespace QuestDialogueSystem
{
    [DefaultExecutionOrder(-1000)]
    public class Locator : MonoBehaviour
    {
        [SerializeField] MonoBehaviour dialogueRunnerReference;
        [SerializeField] MonoBehaviour questManager;
        [SerializeField] MonoBehaviour notificationUI;
        [SerializeField] MonoBehaviour inventory;
        public static IDialogueRunner DialogueRunner{get; private set;}
        public static QuestManager QuestManager{get; private set;}
        public static INotification NotificationUI{get; private set;}
        public static IInventory Inventory{get; private set;}

        void Awake()
        {
            InitializeDialogueRunner();
            InitializeQuestManager();
            InitializeNoficationUI();
            InitializeInventory();
        }

        void InitializeDialogueRunner()
        {
            if (dialogueRunnerReference is IDialogueRunner runner)
            {
                DialogueRunner = runner;
                if(DialogueRunner != null)Debug.Log("DialogueRunner initialized successfully");
            } else 
            {
                Debug.LogError("[Locator] Assigned reference does not implement IDialogueRunner.");
            }
        }

        void InitializeQuestManager()
        {
            if (questManager is QuestManager qm)
            {
                QuestManager = qm;
                if(QuestManager != null) Debug.Log("QuestManager initialized successfully");
            } else 
            {
                Debug.LogError("[Locator] Assigned reference is not a QuestManager.");
            }
        }

        void InitializeNoficationUI()
        {
            if (notificationUI is INotification n)
            {
                NotificationUI = n;
                if(NotificationUI != null) Debug.Log("NotificationUI initialized successfully");
            } else 
            {
                Debug.LogError("[Locator] Assigned reference is not a INotification.");
            }
        }

        void InitializeInventory()
        {
            if (inventory is IInventory i)
            {
                Inventory = i;
                if(Inventory != null) 
                {
                    Inventory.Initialize();
                    Debug.Log("Inventory initialized successfully");
                }                

            } else 
            {
                Debug.LogError("[Locator] Assigned reference is not an Inventory.");
            }
        }
    }
}