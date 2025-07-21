using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Transform muzzle;

    public bool autoReload;
    [SerializeField] int damage;
    [SerializeField] float maxShotDistance;
    [SerializeField] int magzineSize;
    [SerializeField] int reserveAmmo;
    int magazineAmmo;
    [SerializeField] float fireCooldown;
    float fireCountdown;

    [SerializeField] float fireShakeDuration;
    [SerializeField] float fireShakeMagnitude;

    public bool CanFire => fireCountdown <= 0 && magazineAmmo > 0;
    public bool MustReload {get; private set;}

    void Awake()
    {
        var actionStates = GetComponentInParent<ActionStates>();
        actionStates.Select(this);

        muzzle = transform.GetChild(0);
    }

    void Start()
    {
        RaycastShotController.Instance.SetGunType("handgun");
        Initialize();
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

        muzzle = transform.GetChild(0);
    }

    public void Fire()
    {
        if(magazineAmmo == 0)
        {
            if(reserveAmmo != 0) Debug.Log("Need Reload");
            else Debug.Log("Ammo Depleted");
        }

        if(!CanFire) return;

        magazineAmmo -= 1;

        RaycastShot();
        CameraShaker.Instance.Shake(fireShakeDuration, fireShakeMagnitude);

        fireCountdown = fireCooldown;
        Debug.Log($"Current Ammo ({magazineAmmo}/{reserveAmmo})");
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

        Debug.Log($"Current Ammo ({magazineAmmo}/{reserveAmmo})");
    }

    void Update()
    {
        if(fireCountdown > 0) fireCountdown -= Time.deltaTime; 
    }
}