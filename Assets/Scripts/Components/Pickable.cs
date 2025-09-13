using QuestDialogueSystem;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour 
{
    [HideInInspector] public ItemStack stack;
    [SerializeField] ItemData item;
    [SerializeField] int count = 1;
    [HideInInspector] public ItemManager Manager;

    void UseCustomSetting() 
    {   
        if(stack.IsEmpty)
        {
            stack = new ItemStack(item, count);
            if(ItemManager.Instance != null) Manager = ItemManager.Instance;
        }    
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(stack.IsEmpty) UseCustomSetting();
            
            if (ItemManager.Instance.TryPickItem(stack, transform.position))
            {
                gameObject.SetActive(false);
            }
        }
    }
}