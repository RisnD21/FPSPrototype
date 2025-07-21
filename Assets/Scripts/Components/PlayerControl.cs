using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(ActionStates))]
public class PlayerControl : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D rb;
    
    ActionStates actionStates;

    [SerializeField] float maxSpeed = 4.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float tolerance = 0.1f;
    [SerializeField] float turnSpeed = 1.0f;
    Vector2 targetSpeed;
    
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        actionStates = GetComponent<ActionStates>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        Aim();
        Reload();
        Fire();
    }

    private void Move()
    {
        Vector2 inputVector = playerInput.InputVector;

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
        }    
    } 
}
