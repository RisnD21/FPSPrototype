using QuestDialogueSystem;
using Unity.VisualScripting;
using UnityEngine;


[DefaultExecutionOrder(-1001)]
public class GameManager : MonoBehaviour
{
    ItemCounter counter;
    [SerializeField] InventoryUI inventoryUI;

    void Awake()
    {
        counter = GetComponent<ItemCounter>();
        
    }

    async void Start()
    {
        VFXManager.Instance.Initialize();
        DamageTextManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        inventoryUI.Initialize();
        await StateIconLoader.LoadAsync();
    }  
}