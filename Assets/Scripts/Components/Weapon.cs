using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Transform muzzle;
    Transform flash;

    public bool autoReload;
    [SerializeField] string gunType;
    [SerializeField] int damage;
    [SerializeField] float maxShotDistance;
    [SerializeField] int magzineSize;
    [SerializeField] int reserveAmmo;
    int magazineAmmo;
    [SerializeField] float fireCooldown;
    float fireCountdown;

    [SerializeField] float fireShakeDuration;
    [SerializeField] float fireShakeMagnitude;

    AmmoMonitor ammoMonitor;

    public bool CanFire => fireCountdown <= 0 && magazineAmmo > 0;
    public bool MustReload {get; private set;}
    public bool AmmoDepleted {get; private set;}

    bool inDebugMode;

    void Awake()
    {
        var actionStates = GetComponentInParent<ActionStates>();
        actionStates.Select(this);

        muzzle = transform.GetChild(0);
        flash = transform.GetChild(1);
    }

    void Start()
    {
        RaycastShotController.Instance.SetGunType(gunType);
        // Initialize();
        Reload();
    }

    public void Initialize()
    {
        //we set this manually in early stage of development
        damage = 5;

        magzineSize = 7;
        reserveAmmo = 28;
        fireCooldown = 0.5f;
        maxShotDistance = 50.0f;

        fireShakeDuration = 0.05f;
        fireShakeMagnitude = 0.05f;
    }

    public void RegisterMonitor (AmmoMonitor m)
    {
        ammoMonitor = m;
        UpdateAmmoMonitor(magazineAmmo, reserveAmmo);
    }
    void UpdateAmmoMonitor(int current, int reserved)
    {
        if(ammoMonitor == null) return;
        ammoMonitor.SyncAmmo(current, reserved);
    }
    

    public void Fire()
    {
        if(magazineAmmo == 0)
        {
            if(reserveAmmo != 0) 
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
        CameraShaker.Instance.Shake(fireShakeDuration, fireShakeMagnitude);

        fireCountdown = fireCooldown;
        if(inDebugMode) Debug.Log($"Current Ammo ({magazineAmmo}/{reserveAmmo})");
        UpdateAmmoMonitor(magazineAmmo, reserveAmmo);
    }

    void RaycastShot()
    {
        Vector3 origin = muzzle.position;
        origin.z = 0;

        Vector3 direction = muzzle.right; 
        direction.z = 0;

        int mask = LayerMask.GetMask("NPC", "Obstacle", "Player");
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
        if (reserveAmmo == 0)
        {
            return;
        }

        int ammoNeed = magzineSize - magazineAmmo;
        int ammoToFill = Mathf.Min(reserveAmmo, ammoNeed);

        reserveAmmo -= ammoToFill;
        magazineAmmo += ammoToFill;

        MustReload = false;
        if(inDebugMode) Debug.Log($"Current Ammo ({magazineAmmo}/{reserveAmmo})");
        UpdateAmmoMonitor(magazineAmmo, reserveAmmo);
    }

    void Update()
    {
        if(fireCountdown > 0) fireCountdown -= Time.deltaTime; 
    }
}