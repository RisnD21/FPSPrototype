using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDamageHandler
{
    // 回傳剩餘要往下傳的傷害
    int HandleDamage(int amount);
}

public class DamagePipeline : MonoBehaviour
{
    [SerializeField] GameObject[] handlerObjects;
    [SerializeField] Damageable body;
    List<IDamageHandler> _handlers;

    void Awake()
    {
        _handlers = new();
        if(handlerObjects.Length == 0) return;

        foreach (var handlerGO in handlerObjects)
        {
            if(handlerGO.TryGetComponent<IDamageHandler>(out var handler))
                _handlers.Add(handler);
        }
    }

    public void TakeDamage(int amount)
    {
        foreach (var h in _handlers) amount = h.HandleDamage(amount);
        if(amount > 0) body.TakeDamage(amount);
    }
}