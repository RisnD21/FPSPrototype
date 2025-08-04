using System;
using System.Collections.Generic;
using QuestDialogueSystem;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(ActionStates))]
public class PlayerControl : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D rb;
    
    ActionStates actionStates;

    [SerializeField] float walkSpeed = 9.0f;
    [SerializeField] float sprintSpeed = 16.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float tolerance = 0.1f;
    [SerializeField] float turnSpeed = 1.0f;
    [SerializeField] AmmoMonitor ammoMonitor;
    [SerializeField] List<GameObject> weapons;
    
    bool rifleUnlock;

    float interactDistance = 5f;
    Vector2 boxSize = new Vector2(5f, 4f); // BoxCast 區域大小
    [SerializeField] LayerMask interactableLayer;

    Vector2 targetSpeed;
    
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        actionStates = GetComponent<ActionStates>();

        foreach (var weapon in weapons) weapon.SetActive(false);
    }

    void SelectWeapon()
    {
        int index = playerInput.weaponIndex;
        if (index < 0) return;
        
        playerInput.weaponIndex = -1; // 重設 weaponIndex，避免重複切換

        if (!IsWeaponSelectable(index)) return;

        var weapon = weapons[index].GetComponent<Weapon>();
        if (actionStates.currentWeapon == weapon) return;

        weapon.RegisterMonitor(ammoMonitor);
        actionStates.Select(weapon);
    }

    bool IsWeaponSelectable(int index)
    {
        // 無效 index
        if (index < 0 || index >= weapons.Count) return false;

        // 未解鎖條件
        if (index == 1 && !rifleUnlock) return false;

        return true;
    }

    public void UnlockRifle() => rifleUnlock = true;

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        Aim();
        Reload();
        Fire();
        Interact();
        SelectWeapon();
    }

    private void Move()
    {
        Vector2 inputVector = playerInput.InputVector;

        float maxSpeed = playerInput.IsSprint ? sprintSpeed : walkSpeed;

        targetSpeed = inputVector.magnitude < tolerance ? Vector2.zero : maxSpeed * inputVector;

        Vector2 currentSpeed = rb.linearVelocity;    

        if ((currentSpeed - targetSpeed).magnitude < tolerance)
        {
            rb.linearVelocity = targetSpeed;
        } else
        {
            rb.linearVelocityX = Mathf.Lerp(currentSpeed.x, targetSpeed.x, Time.fixedDeltaTime * acceleration);
            rb.linearVelocityY = Mathf.Lerp(currentSpeed.y, targetSpeed.y, Time.fixedDeltaTime * acceleration);
        }
        
        actionStates.OnMove(currentSpeed != Vector2.zero);
    }

    void Aim()
    {
        if(playerInput.IsDialogueMode) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; 

        Vector3 playerPos = transform.position;
        Vector2 direction = mouseWorldPos - playerPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Reload()
    {
        if (playerInput.IsReload)
        {
            actionStates.Reload();
            playerInput.IsReload = false;
        }    
    }

    void Fire()
    {
        if (playerInput.IsFire)
        {
            actionStates.Fire();
            playerInput.IsFire = false;
        } else actionStates.StopFire();
    } 

    void Interact()
    {
        if(!playerInput.IsInteract) return;

        Debug.Log("Trying to interact");
        Vector2 origin = (Vector2)transform.position;
        Vector2 direction = transform.right; // ↑ 如果角色前方是 Vector2.up
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, angle, direction, interactDistance, interactableLayer);
        if (hit.collider != null)
        {
            if(hit.collider.TryGetComponent<QuestNarrator>(out var narrator))
            {
                Locator.DialogueRunner.SetPlayerInput(playerInput);
                narrator.Interact();
            }
        }
        playerInput.IsInteract = false;
    }
}
