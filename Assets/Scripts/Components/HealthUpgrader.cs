using UnityEngine;

public class HealthUpgrader : MonoBehaviour
{
    [SerializeField] Damageable body;

    public void ApplyFirstUpgrade()
    {
        body.SetMaxHealth(30);
    }

    public void ApplySecondUpgrade()
    {
        body.SetMaxHealth(50);
    }
}