using QuestDialogueSystem;
using Unity.VisualScripting;
using UnityEngine;


[DefaultExecutionOrder(-1001)]
public class GameManager : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;

    async void Start()
    {
        VFXManager.Instance.Initialize();
        DamageTextManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        inventoryUI.Initialize();
        await StateIconLoader.LoadAsync();
    }  
}