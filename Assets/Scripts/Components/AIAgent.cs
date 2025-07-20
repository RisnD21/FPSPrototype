using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.U2D;
using System;

public class AIAgent : MonoBehaviour
{
    AIPath path;
    
    public GameObject player;
    public float walkSpeed;
    public float chaseSpeed;
    public List<Vector3> PatrolWaypoints;

    void Awake()
    {
        path = GetComponent<AIPath>();
    }

    void Start()
    {
        path.maxSpeed = walkSpeed;
        StartCoroutine(LookAt(player.transform.position));
    }

    
    IEnumerator Observe(Vector3 point, float duration)
    {
        yield return StartCoroutine(LookAt(point)); //StartCoroutine 完成後才會繼續往下
        while(duration > 0 || !playerInSight())
        {
            //randomize a offset from pos every 3 second to simulate reality
        }
    }

    IEnumerator LookAt(Vector3 point)
    {
        float duration = 1.0f;

        Vector3 direction = point - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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

    bool playerInSight()
    {
        //return true if player is in sightAngle and has no obstacle between sight
        return true;
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