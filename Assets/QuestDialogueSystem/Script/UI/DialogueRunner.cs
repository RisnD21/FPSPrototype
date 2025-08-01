using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace QuestDialogueSystem
{
    public class DialogRunner : MonoBehaviour, IDialogueRunner
    {
        [Header("UI Reference")]
        [SerializeField] GameObject dialogPanel;
        [SerializeField] TextMeshProUGUI dialogText;
        [SerializeField] Image portraitImage;
        [SerializeField] Sprite defaultPortraitImage;
        [SerializeField] TextMeshProUGUI portraitDescription;

        [SerializeField] Transform optionsParent;
        [SerializeField] GameObject optionPrefab;

        [Header("Current State")]
        [SerializeField] ConversationScript currentScript;
        ConversationPiece currentPiece;

        bool isDebugMode = false;

        void Awake()
        {
            if (dialogPanel == null)
                Debug.LogError("DialogueRunner: dialogPanel is not assigned.");

            if (dialogText == null)
                Debug.LogError("DialogueRunner: dialogText is not assigned.");

            if (optionsParent == null)
                Debug.LogError("DialogueRunner: optionsParent is not assigned.");

            if (optionPrefab == null)
                Debug.LogError("DialogueRunner: optionButtonPrefab is not assigned.");

            if (portraitImage == null)
                Debug.LogError("DialogueRunner: portraitImage not assigned.");

            // Optional: 預設關閉對話面板
            dialogPanel.SetActive(false);
        }

        void InitializeOptions()
        {
            foreach (Transform child in optionsParent)
            {
                Destroy(child.gameObject);
            }
        }

        public void StartConversation(ConversationScript script)
        {
            if(isDebugMode) Debug.Log("Starting Conversation");
            currentScript = script;
            ShowPiece(script.GetFirstPiece());
        }

        void ShowPiece(ConversationPiece piece)
        {
            dialogPanel.SetActive(true);
            if(isDebugMode) Debug.Log("Showing Dialog");

            currentPiece = piece;

            UpdateDialog(piece.text);
            
            Sprite newPortrait = currentPiece.portrait != null ? 
                currentPiece.portrait : defaultPortraitImage;
            UpdatePortraitImage(newPortrait);

            string description = piece.portraitDescription ?? "Unknown";
            UpdatePortraitDescription(description);

            if (piece.audio != null) PlayAudio(piece.audio);


            InitializeOptions();

            foreach (var option in piece.options)
            {
                if (option != null)
                {
                    var optionUnit = Instantiate(optionPrefab, optionsParent);
                    var optionText = optionUnit.GetComponentInChildren<TextMeshProUGUI>();
                    optionText.text = option.text;
                    var optionButton = optionUnit.GetComponent<Button>();

                    optionButton.onClick.AddListener(() => HandleOption(option));
                }
            }
        }

        void UpdatePortraitImage(Sprite image)
        {
            portraitImage.sprite = image;
        }

        void UpdatePortraitDescription(string text)
        {
            portraitDescription.text = text;
        }

        void UpdateDialog(string text)
        {
            dialogText.text = text;
        }

        void PlayAudio(AudioClip audio)
        {
            
        }

        void HandleOption(ConversationOption option)
        {
            if(isDebugMode) Debug.Log("An option is selected");

            if (!string.IsNullOrEmpty(option.targetID))
            {
                ConversationPiece nextPiece = currentScript.GetPieceByID(option.targetID);
                ShowPiece(nextPiece);
            } else
            {
                ResetDialog();
            }

            option.onSelected?.Invoke();
        }

        void ResetDialog()
        {
            if(isDebugMode) Debug.Log("Dialog Reset");
            UpdateDialog("");
            UpdatePortraitImage(defaultPortraitImage);
            UpdatePortraitDescription("");
            InitializeOptions();
            dialogPanel.SetActive(false);
        }
    }    
}

