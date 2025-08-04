namespace QuestDialogueSystem
{
    public interface IDialogueRunner
    {
        void StartConversation(ConversationScript script);
        void SetPlayerInput(PlayerInput input); //DialogueRunner should toggle conversation mode
        void ResetDialog();
    }
}