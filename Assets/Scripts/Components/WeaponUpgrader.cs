using UnityEngine;

public class WeaponUpgrader : MonoBehaviour
{
    [SerializeField] Weapon handgun;
    [SerializeField] Weapon rifle;

    public void ApplyFirstUpgrade()
    {
        handgun.SetAccuracyDecreaseSpeed(0.25f);
        rifle.SetAccuracyDecreaseSpeed(0.01f);
    }

    public void ApplySecondUpgrade()
    {
        handgun.SetAccuracyRecoverSpeed(0.1f);
        rifle.SetAccuracyRecoverSpeed(0.1f);
    }
}