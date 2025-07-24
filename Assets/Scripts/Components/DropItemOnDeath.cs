using Unity.VisualScripting;
using UnityEngine;

public class DropItemOnDeath : MonoBehaviour
{
    [SerializeField] string ItemID;
    public void Drop()
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        ItemManager.Instance.SpawnItem(ItemID, pos);
    }
}
