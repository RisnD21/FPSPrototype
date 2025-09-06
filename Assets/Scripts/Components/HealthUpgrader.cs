using UnityEngine;

public class HealthUpgrader : MonoBehaviour
{
    [SerializeField] Damageable body;
    [SerializeField] int firstUpgrade = 30;
    [SerializeField] int secondUpgrade = 50;

    public void ApplyFirstUpgrade()
    {
        body.SetMaxHealth(25);
    }

    public void ApplySecondUpgrade()
    {
        body.SetMaxHealth(50);
    }
}