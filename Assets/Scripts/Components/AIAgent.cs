using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;
using UnityEditorInternal;

public class AIAgent : MonoBehaviour
{
    AIPath path;
    
    public Transform player;
    public LayerMask obstacleMask;
    public float walkSpeed;
    public float chaseSpeed;
    public List<Transform> patrolWaypoints;

    public float viewAngle = 120.0f;

    public Vector3 lastSeenPlayerPos;
    Vector3 destination;
    
    public bool isDebugMode = false;
    
    public void OnHit() => beingHit = true;    
    [HideInInspector] public bool beingHit;

    public bool HasReachDestination()
    {
        if (Vector2.Distance(destination, transform.position) < 2f) 
        {
            if (isDebugMode) Debug.Log($"{gameObject.name} has reach destination");
            return true;
        }
        return false;
    }

    public IState patrolling;
    public IState attacking;
    public IState chasing;
    public IState searching;
    public IState currentState;

    void Awake()
    {
        path = GetComponent<AIPath>();
    }

    void Start()
    {
        Initialize();
        TransitionTo(patrolling);
    }

    void Initialize()
    {
        InitializeStates();
        path.maxSpeed = walkSpeed;
    }

    void InitializeStates()
    {
        patrolling = new Patrolling(this);
        attacking = new Attacking(this);
        chasing = new Chasing(this);
        searching = new Searching(this);
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
    public IEnumerator LookAt(Vector3 point, float duration = 0.5f)
    {
        if (isDebugMode) Debug.Log($"{gameObject.name} is looking at {point}");

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
        if(player == null) return false;

        //is player in viewAngle?
        Vector2 dirToPlayer = player.position - transform.position;
        float angle = Vector2.Angle(dirToPlayer, transform.up); //face up
        if (viewAngle/2f < angle) return false;

        //is there obstalce between sight?
        float distanceToPlayer = dirToPlayer.magnitude;
        RaycastHit2D hit 
        = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);
        if (hit.collider != null) return false;

        //too far, can't see
        if(distanceToPlayer > 20f) return false;
        return true;
    }

    public void TransitionTo(IState state)
    {
        if(currentState != null && currentState == state) return;
        
        currentState?.OnExit();
        currentState = state;
        state.OnEnter();
    }

    public bool TryMoveTo(Vector3 targetPos, float speed = 0)
    {
        targetPos.z = transform.position.z;

        path.maxSpeed = speed == 0 ? walkSpeed : speed;

        if (isDebugMode) Debug.Log($"{gameObject.name} is thinking how to reach {destination}");

        NNConstraint constraint = NNConstraint.Default;
        constraint.constrainWalkability = true;
        constraint.walkable = true;

        var fromNode = AstarPath.active.GetNearest(transform.position, constraint).node;
        var toInfo = AstarPath.active.GetNearest(targetPos, constraint);
        var toNode = toInfo.node;

        if (fromNode != null && toNode != null && PathUtilities.IsPathPossible(fromNode, toNode))
        {
            destination = toInfo.position;
            path.destination = destination;

            if (isDebugMode) Debug.Log($"{gameObject.name} is moving to {destination}");
            return true;
        }
        else
        {
            if (isDebugMode) Debug.LogWarning($"{gameObject.name} can't reach {targetPos}");
            return false;
        }
    }

    public void Halt() => path.destination = transform.position;

    void Update()
    {
        currentState?.OnUpdate();
    }
}