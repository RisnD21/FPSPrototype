using QuestDialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    ItemCounter counter;
    [SerializeField] InventoryUI inventoryUI;

    void Awake()
    {
        counter = GetComponent<ItemCounter>();
    }
    void Start()
    {
        VFXManager.Instance.Initialize();
        DamageTextManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        counter.Initialize();
        inventoryUI.Initialize();
    }  
}