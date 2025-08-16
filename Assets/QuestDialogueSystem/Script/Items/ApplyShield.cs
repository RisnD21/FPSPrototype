
using QuestDialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "GameJam/Item/ItemAction/ApplyShield")]

public class ApplyShield : ItemAction
{
    public int maxHealth;
    public float regenSpeed;
    public float rechargeCooldown;

    public override bool TryUse(UseContext useContext, InventorySlot slot = null)
    {
        GameObject shield = useContext.user;
        Damageable shieldBody = shield.GetComponent<Damageable>();
        ShieldDamageHandler shieldProperty = shield.GetComponent<ShieldDamageHandler>();

        if(shieldBody.maxHealth > maxHealth) return false;
        
        shieldBody.SetMaxHealth(maxHealth);
        shieldProperty.SetProperty(regenSpeed, rechargeCooldown);
        return true;
    }
}