using QuestDialogueSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    ItemCounter counter;

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

    }  
}