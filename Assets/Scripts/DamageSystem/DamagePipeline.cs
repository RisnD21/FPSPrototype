using System.Linq;
using UnityEngine;

public class DamagePipeline : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] handlers;
    [SerializeField] Damageable body;
    IDamageHandler[] _handlers;

    void Awake() => _handlers = handlers?.OfType<IDamageHandler>().ToArray();

    public void TakeDamage(int amount)
    {
        foreach (var h in _handlers) amount = h.HandleDamage(amount);
        if(amount > 0) body.TakeDamage(amount);
    }
}