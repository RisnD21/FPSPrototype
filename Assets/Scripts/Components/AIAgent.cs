using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;

public class AIAgent : MonoBehaviour
{
    AIPath path;
    
    public Transform player;
    public LayerMask obstacleMask;
    public float walkSpeed;
    public float chaseSpeed;
    public List<Transform> patrolWaypoints;

    public float viewAngle = 120.0f;
    
    bool isDebugMode = true;



    IState patrolling;
    IState attacking;
    IState currentState;

    void Awake()
    {
        path = GetComponent<AIPath>();
    }

    void Start()
    {
        Initialize();
        InitializeStates();

        TransitionTo(patrolling);
    }

    void Initialize()
    {
        
        path.maxSpeed = walkSpeed;
    }

    void InitializeStates()
    {
        patrolling = new Patrolling(this);
        attacking = new Attacking(this);
    }

    public IEnumerator Observe(Vector3 point, float duration = 5f)
    {
        if (isDebugMode) Debug.Log($"{gameObject.name} is observing {point}");
        yield return StartCoroutine(LookAt(point)); //StartCoroutine 完成後才會繼續往下
    
        while(duration > 0)
        {
            //randomize a offset from pos every 3 second to simulate reality
            duration -= Time.deltaTime;
            yield return null;
        }
    }

    //Face at point
    public IEnumerator LookAt(Vector3 point)
    {
        if (isDebugMode) Debug.Log($"{gameObject.name} is looking at {point}");
        float duration = 0.5f;

        Vector3 direction = point - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, targetAngle);
        
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
    }

    public bool IsPlayerInSight()
    {
        //is player in viewAngle?
        Vector2 dirToPlayer = player.position - transform.position;
        float angle = Vector2.Angle(dirToPlayer, transform.up); //face up
        if (viewAngle/2f < angle) return false;

        //is there obstalce between sight?
        float distanceToPlayer = dirToPlayer.magnitude;
        RaycastHit2D hit 
        = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);
        if (hit.collider != null) return false;

        if (isDebugMode) Debug.Log($"{gameObject.name} has spotted player");
        return true;
    }

    public void TransitionTo(IState state)
    {
        if (currentState != null)
        {
            if (isDebugMode) Debug.Log($"{gameObject.name} decides to change from {currentState} to {state}");
        }else{
            if (isDebugMode) Debug.Log($"{gameObject.name} change state to {state}");
        }

        currentState?.OnExit();
        currentState = state;
        state.OnEnter();
    }

    public void MoveTo(Vector3 point, float speed = 0)
    {
        if (isDebugMode) Debug.Log($"{gameObject.name} is moving to {point}");
        point.z = transform.position.z;

        path.maxSpeed = speed == 0? walkSpeed : speed;
        path.destination = point;
    }

    public bool HasReachPosition(Vector3 point)
    {
        return Vector3.Distance(transform.position,  point) < 0.5f;
    }

    void Update()
    {
        if (IsPlayerInSight()) StartCoroutine(LookAt(player.transform.position));
        // if (IsPlayerInSight()) TransitionTo(new Attacking(this));   
    }

    //change below to new structure
    // void Chase()
    // {
    //     //record playerpos as lastSeenPos
    //     //if player in attack range, call attack
    //     path.maxSpeed = chaseSpeed;
    //     //move to lastSeenPos
    // }

    // void Attack()
    // {
    //     isAiming = true;
    //     //face at player
    //     //I'll implement fire part
    // }

    // void MoveTo(Vector3 pos)
    // {
    //     path.destination = pos;
    // }

    // void Update()
    // {
    //     if (playerIsDead) return;

    //     if (isAiming) return;
    //     if (playerInSight() ) Chase();

    // }
}


//是否在視野內：
// 1. Raycast 判斷有無障礙物
// 2. 判斷是否在可視範圍
// Vector3 dirToTarget = (target.position - eye.position).normalized;
// if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) 在視線中！
// 前往指定地點方式： path.destination = target.position;
// 設定速度的方式：path.maxSpeed

// 邏輯：
// idle：不定時選定方向，方向請於 inspector 中設定
// patrol：定時前往特定地點，地點請於 inspector 中設定
// check playInSight each frame
// chase：若玩家在視線中，記錄當前位置，前進直到進入射程，進入攻擊模式；否則前往最後紀錄位置
// attack：若玩家在射程內，則開火；否則進入追擊模式
// search：往玩家位置看去，若玩家在視線中，進入chase；否則紀錄該方向並微幅隨機張望，10秒後回到待命地點