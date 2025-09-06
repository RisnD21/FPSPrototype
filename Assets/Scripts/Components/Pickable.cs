using QuestDialogueSystem;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour 
{
    [HideInInspector] public ItemStack stack;
    [HideInInspector] public ItemManager Manager;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ItemManager.Instance.TryPickItem(stack, transform.position))
            {
                Debug.Log($"You've collected {stack}");
                gameObject.SetActive(false);
            }
        }
    }
}