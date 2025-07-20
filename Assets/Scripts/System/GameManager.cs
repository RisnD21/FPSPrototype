using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        PoolSystem.Instance.Initialize();
        VFXManager.Instance.Initialize();
        DamageTextManager.Instance.Initialize();
    }  
}