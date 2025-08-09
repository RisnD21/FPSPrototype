using System.Runtime.InteropServices.WindowsRuntime;
using QuestDialogueSystem;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Transform muzzle;
    Transform flash;

    public bool autoReload;
    [SerializeField] public string gunType;
    [SerializeField] ItemData ammoType;
    [SerializeField] int damage;
    [SerializeField] float maxShotDistance;
    [SerializeField] int magzineSize;
    IInventory inventory;

    int ReserveAmmo
    {
        get
        {
            if (inventory == null) Debug.LogError("[Weapon] inventory is null on " + GetComponentInParent<Transform>().gameObject.name);

            return inventory.Count(ammoType);
        }
        set
        {
            int ammoBefChange = inventory.Count(ammoType);
            int ammoToRemove  = ammoBefChange - value;
            inventory.TryRemove(ammoType, ammoToRemove, out _);
        }
    }
    int magazineAmmo;
    [SerializeField] float fireCooldown;
    float fireCountdown;

    [SerializeField] bool canShakeCam;
    [SerializeField] float fireShakeDuration;
    [SerializeField] float fireShakeMagnitude;

    AmmoMonitor ammoMonitor;

    public bool CanFire => fireCountdown <= 0 && magazineAmmo > 0;
    public bool MustReload {get; private set;}
    public bool AmmoDepleted {get; private set;}
    public bool hasRegister;

    bool inDebugMode = false;

    void Awake()
    {
        muzzle = transform.GetChild(0);
        flash = transform.GetChild(1);
    }

    void OnDisable() => hasRegister = false;

    void Start()
    {
        RaycastShotController.Instance.SetGunType(gunType);
    }

    public void LoadAmmo(IInventory inventory)
    {
        this.inventory = inventory;
        Reload();
    }

    public void RegisterMonitor (AmmoMonitor m)
    {
        ammoMonitor = m;
        UpdateAmmoMonitor(magazineAmmo, ReserveAmmo);
        hasRegister = true;
    }
    void UpdateAmmoMonitor(int current, int reserved)
    {
        if(ammoMonitor == null) return;
        ammoMonitor.SyncAmmo(ammoType, current, reserved);
    }
    

    public void Fire()
    {
        if(magazineAmmo == 0)
        {
            if(ReserveAmmo != 0) 
            {
                MustReload = true;
                if(inDebugMode) Debug.Log("Need Reload");
            }
            else 
            {
                if(inDebugMode) Debug.Log("Ammo Depleted");
                AmmoDepleted = true;
            }
        }

        if(!CanFire) return;
        magazineAmmo -= 1;
        flash.GetComponent<ParticleSystem>().Play();
        RaycastShot();

        if(canShakeCam) CameraShaker.Instance.Shake(fireShakeDuration, fireShakeMagnitude);

        fireCountdown = fireCooldown;
        if(inDebugMode) Debug.Log($"Current Ammo ({magazineAmmo}/{ReserveAmmo})");
        UpdateAmmoMonitor(magazineAmmo, ReserveAmmo);
    }

    void RaycastShot()
    {
        Vector3 origin = muzzle.position;
        origin.z = 0;

        Vector3 direction = muzzle.right; 
        direction.z = 0;

        int mask = LayerMask.GetMask("NPC", "Obstacle", "Player", "Shield");
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxShotDistance, mask);

        Vector2 endPoint;

        if(hit.collider != null)
        {
            VFXManager.Instance.SpawnImpactEffect(hit.point, hit.normal, hit.collider.tag);
            endPoint = hit.point;

            if(hit.collider.TryGetComponent<Damageable>(out var target))
            {
                target.TakeDamage(damage);
            }
        } else
        {
            endPoint = origin + direction * maxShotDistance;
        }

        RaycastShotController.Instance.AddTrail(muzzle.position, endPoint);
    }


    public void Reload()
    {
        if (ReserveAmmo == 0)
        {
            return;
        }

        int ammoNeed = magzineSize - magazineAmmo;
        int ammoToFill = Mathf.Min(ReserveAmmo, ammoNeed);

        ReserveAmmo -= ammoToFill;
        magazineAmmo += ammoToFill;

        MustReload = false;
        if(inDebugMode) Debug.Log($"Current Ammo ({magazineAmmo}/{ReserveAmmo})");
        UpdateAmmoMonitor(magazineAmmo, ReserveAmmo);
    }

    void Update()
    {
        if(fireCountdown > 0) fireCountdown -= Time.deltaTime; 
    }
}