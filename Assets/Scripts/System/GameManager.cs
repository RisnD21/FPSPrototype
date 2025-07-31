using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        VFXManager.Instance.Initialize();
        DamageTextManager.Instance.Initialize();
        ItemManager2.Instance.Initialize();
    }  
}