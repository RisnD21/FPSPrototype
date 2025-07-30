using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour 
{

    [HideInInspector] public string Id;
    [HideInInspector] public ItemManager Manager;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You've collect a fresh soul");
            gameObject.SetActive(false);
        }
    }
}