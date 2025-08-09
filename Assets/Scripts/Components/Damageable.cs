using UnityEngine;
using UnityEngine.Animations;

public class Damageable : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] HealthBar healthBar;
    [SerializeField] Transform damageTextPos;
    AIAgent agent;
    int currentHealth;
    bool hasInitialize;

    DropItemOnDeath dropHelper;
    void Awake()
    {
        agent = GetComponentInParent<AIAgent>();
        dropHelper = GetComponent<DropItemOnDeath>();
    }

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

        if(agent != null) agent.OnHit();

        if(currentHealth != 0) return;

        if(gameObject.CompareTag("Shield"))
        {
            
        }
        else KillObject();
    }

    void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }

    public void KillObject()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }
        if( gameObject!= null) Destroy(gameObject);

        if(dropHelper != null) dropHelper.Drop();
    }
}