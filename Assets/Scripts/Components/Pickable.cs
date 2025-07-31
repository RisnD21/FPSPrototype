using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour 
{

    [HideInInspector] public string Id;
    [HideInInspector] public ItemManager2 Manager;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ItemManager2.Instance.PickItem(Id, transform.position);
            gameObject.SetActive(false);
        }
    }
}