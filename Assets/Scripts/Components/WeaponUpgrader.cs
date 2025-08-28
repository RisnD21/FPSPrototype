using UnityEngine;

public class WeaponUpgrader : MonoBehaviour
{
    [SerializeField] Weapon handgun;
    [SerializeField] Weapon rifle;

    public void ApplyFirstUpgrade()
    {
        handgun.SetAccuracy(1);
        rifle.SetAccuracy(0.95f);
    }

    public void ApplySecondUpgrade()
    {
        rifle.SetAccuracy(1);
        rifle.SetAccuracyDecreaseSpeed(0.01f);
    }
}