using UnityEngine;
using UnityEngine.Animations;

public class Damageable : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] HealthBar healthBar;
    [SerializeField] Transform damageTextPos;
    int currentHealth;
    bool hasInitialize;

    void Start()
    {
        Heal(maxHealth);
        healthBar.SetMaxHealth(maxHealth);
        hasInitialize = true;
    }

    public void Heal(int value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

        if(healthBar != null)
            UpdateHealthBar();

        if(damageTextPos != null && hasInitialize)
            DamageTextManager.Instance.SpawnText(value, damageTextPos.position);
    }

    public void TakeDamage(int value)
    {
        currentHealth = Mathf.Clamp(currentHealth - value, 0, maxHealth);

        if(healthBar != null)
            UpdateHealthBar();

        if(damageTextPos != null)
            DamageTextManager.Instance.SpawnText(-value, damageTextPos.position);

        if(currentHealth == 0) KillObject();
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
        Debug.Log($"Soilder, Health({currentHealth}/{maxHealth})");
    }

    public void KillObject()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }
    }
}