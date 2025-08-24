using QuestDialogueSystem;
using UnityEngine;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEngine.InputSystem.XR.Haptics;
using Unity.Cinemachine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Transform flash;

    public bool autoReload;
    [SerializeField] public string gunType;
    [SerializeField] ItemData ammoType;
    [SerializeField] int damage;
    [SerializeField] float fireNoise;
    [SerializeField] float maxShotDistance;
    [SerializeField] int magazineSize;
    [SerializeField] GameObject inventoryObj;


    [SerializeField] float maxAccuracy = 1f;
    float accuracy;
    [SerializeField] float accuracyRecoverPerSec = 0f;
    [SerializeField] float accuracyDecreasePerTrigger = 0f;
    
    [SerializeField] CinemachineImpulseSource impulser;

    [SerializeField] bool canShakeCam;
    [SerializeField] float shakMagnitude;
    
    Coroutine recoverAccuracy;

    public static event Action<Vector3, float> Gunshot;

    public enum FireMode
    {
        single,
        burst,
        auto
    }
    [SerializeField] FireMode firemode;
    int accumulateFire;

    IInventory inventory;

    int ReserveAmmo
    {
        get
        {
            if(inventory == null)
            {
                inventory = inventoryObj.GetComponent<IInventory>();        
            }
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
    [SerializeField] public float fireCooldown;
    float fireCountdown;

    AmmoMonitor ammoMonitor;

    public bool CanFire => fireCountdown <= 0 && magazineAmmo > 0;
    public bool MustReload {get; private set;}
    public bool AmmoDepleted {get; private set;}
    public bool hasRegister;

    bool inDebugMode = false;

    void OnDisable() => hasRegister = false;
    int mask;

    void Awake()
    {
        mask = LayerMask.GetMask("NPC", "Obstacle", "Player", "Shield");
    }

    void Start()
    {
        RaycastShotController.Instance.SetGunType(gunType);
        accuracy = maxAccuracy;
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

        Gunshot?.Invoke(transform.position, fireNoise);

        if(canShakeCam && impulser != null)
        {
            impulser.GenerateImpulseWithVelocity(transform.right.normalized * shakMagnitude);
        }

        SetFireCooldown();        
        ModifyAccuracy();

        if(inDebugMode) Debug.Log($"Current Ammo ({magazineAmmo}/{ReserveAmmo})");
        UpdateAmmoMonitor(magazineAmmo, ReserveAmmo);
    }

    /// <summary>
    /// For NPC fire control – forces cooldown based on accuracy recovery time.
    /// Player-controlled weapons will always be in mode automatic
    /// </summary>
    void SetFireCooldown()
    {
        fireCountdown = fireCooldown;

        if(firemode == FireMode.burst) accumulateFire++;
        if (firemode == FireMode.single || accumulateFire >= 3)
        {
            if(accuracyRecoverPerSec != 0)
                fireCountdown = (maxAccuracy - accuracy)/accuracyRecoverPerSec;
            accumulateFire = 0;
        }
    }

    void ModifyAccuracy()
    {
        accuracy -= accuracyDecreasePerTrigger;
        if(recoverAccuracy != null) StopCoroutine(recoverAccuracy);
        recoverAccuracy = StartCoroutine(RecoverAccuracy());
    }

    IEnumerator RecoverAccuracy()
    {
        while(accuracy < maxAccuracy)
        {
            yield return new WaitForSeconds(0.1f);
            accuracy = Mathf.Min(accuracy + accuracyRecoverPerSec/10, maxAccuracy);
        }
    }   

    void RaycastShot()
    {
        Vector3 origin = muzzle.position;
        origin.z = 0;

        Vector3 direction = muzzle.right + (Vector3)UnityEngine.Random.insideUnitCircle * (1-accuracy); 
        direction.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxShotDistance, mask);

        Vector2 endPoint;

        if(hit.collider != null)
        {
            VFXManager.Instance.SpawnImpactEffect(hit.point, hit.normal, hit.collider.tag);
            endPoint = hit.point;

            if(hit.collider.TryGetComponent<DamageForwarder>(out var target))
            {
                target.ApplyHit(damage);
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

        int ammoNeed = magazineSize - magazineAmmo;
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