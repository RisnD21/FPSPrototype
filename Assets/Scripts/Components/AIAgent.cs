using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using System.Linq;

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
            if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} has reach destination");
            return true;
        }
        return false;
    }

    public IState patrolling;
    //patrolling assigned area
    public IState attacking;
    //if player insight, aim then fire at player
    public IState chasing;
    //if was attacking, then player out of sight, chase until player insight
    public IState searching;
    //if on last seen player position, player out of sight, start observing 
    public IState chatting;
    //on patrolling, npc get bored
    public IState currentState;



    //Alert Behavior
    public bool isAlert;

    //Observe Behavior
    float jitterMagnitude = 1.5f;

    //Chat Behavior
    HashSet<AIAgent> nearbyAllies;
    public AIAgent allyToChat;

    [HideInInspector] public bool isChatting;
    [HideInInspector] public bool needChat;
    [HideInInspector] public float chatDuration;
    [SerializeField] float minChatDuration;
    [SerializeField] float maxChatDuration;
    [SerializeField] float chatTendency = 0.5f;

    void Awake()
    {
        path = GetComponent<AIPath>();
        nearbyAllies = new();
    }

    void OnEnable()
    {
        AICommander.Instance.RequestReport += ReportBack;
    }
    void OnDisable()
    {
        AICommander.Instance.RequestReport -= ReportBack;
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
        chatting = new Chatting(this);
    }

    public IEnumerator Observe(Vector3 point, float duration = 5f)
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is observing {point}");
        yield return StartCoroutine(LookAt(point)); //StartCoroutine 完成後才會繼續往下
    
        float elapsed = 0;
        while(elapsed < duration)
        {
            Vector3 nextView = point + (Vector3) Random.insideUnitCircle * jitterMagnitude;
            yield return StartCoroutine(LookAt(nextView, 0.4f));

            float waitTime = Random.Range(0.5f,3.5f);
            yield return new WaitForSeconds(waitTime);

            elapsed += 0.2f + waitTime;
        }
    }

    //Face at point
    public IEnumerator LookAt(Vector3 point, float duration = 0.3f)
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is looking at {point}");

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
        // if(currentState != null && currentState == state) return;
        
        currentState?.OnExit();
        currentState = state;
        state.OnEnter();
    }

    public bool TryMoveTo(Vector3 targetPos, float speed = 0)
    {
        targetPos.z = transform.position.z;

        path.maxSpeed = speed == 0 ? walkSpeed : speed;

        if (isDebugMode) Debug.Log($"{gameObject.name} is thinking how to reach {targetPos}");

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

            if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is moving to {destination}");
            return true;
        }
        else
        {
            if (isDebugMode) Debug.LogWarning($"{gameObject.name} can't reach {targetPos}");
            return false;
        }
    }

    public void SenseSomething()
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} senses something suspicious");
        if (isDebugMode) Debug.Log("[AIAgent] Updating lastSeenPlayerPos");
        lastSeenPlayerPos = player.position;
        isAlert = true;
    }

    public void BackToNormalState()
    {
        beingHit = false;
        isAlert = false;
    }

    public void CallReinforcement()
    {
        AICommander.Instance.CallReinforcement(this, lastSeenPlayerPos);
    }

    void ReportBack(AIAgent sender)
    {
        if (sender == this) return;
        AICommander.Instance.ReceiveReport(this, transform.position);
    }

    public void Reinforce(Vector3 pos)
    {
        lastSeenPlayerPos = pos;
        isAlert = true;
    }

    public void AddNearbyAllies(AIAgent agent)
    {
        Debug.Log($"[AIAgent] {gameObject.name} add {agent.gameObject.name} to nearbyAllies");
        nearbyAllies.Add(agent);
    }

    public void RemoveNearbyAllies(AIAgent agent)
    {
        Debug.Log($"[AIAgent] {gameObject.name} remove {agent.gameObject.name} from nearbyAllies");
        nearbyAllies.Remove(agent);
    }

    public bool TryFindAllyToChat()
    {
        Debug.Log($"[AIAgent] {gameObject.name} is trying to find someone to chat");
        nearbyAllies.RemoveWhere(a => a == null || Vector3.Distance(transform.position, a.transform.position) > 10f);

        var allyList = nearbyAllies
            .Where(a => CanSee(a.gameObject) && !a.isChatting)
            .ToList();

        if(allyList.Count == 0) return false;
        int indexToPick = Random.Range(0,allyList.Count);
        return TryStartChat(allyList[indexToPick]);
    }

    bool CanSee(GameObject target)
    {
        //obstacle is those that blocks sight
        Vector2 dirToPlayer = target.transform.position - transform.position;
        float distToTarget = dirToPlayer.magnitude;
        RaycastHit2D hit 
        = Physics2D.Raycast(transform.position, dirToPlayer, distToTarget, obstacleMask);
        
        return hit.collider == null;
    }

    bool TryStartChat(AIAgent agent)
    {
        Debug.Log($"[AIAgent] {gameObject.name} is asking if {agent.gameObject.name} want to chat");
        chatDuration = Random.Range(minChatDuration, maxChatDuration);
        
        if (!agent.AcceptToChat(this)) return false;
        allyToChat = agent;
        needChat = true;
        
        return true;
    }

    public bool AcceptToChat(AIAgent ally)
    {
        if(isChatting || Random.Range(0f,1f) > chatTendency)
        {
            Debug.Log($"[AIAgent] {gameObject.name} refuses to chat with {ally.gameObject.name}");
            return false;
        }
        Debug.Log($"[AIAgent] {gameObject.name} accepts to chat with {ally.gameObject.name}");        
        chatDuration = ally.chatDuration;

        Debug.Log($"[AIAgent] {gameObject.name} knows the chat should last for {chatDuration} sec");        
        allyToChat = ally;
        needChat = true;
        return true;
    }

    public void StopChatting()
    {
        Debug.Log($"[AIAgent] {gameObject.name} stop chatting.");
        allyToChat = null;
        isChatting = false;
        needChat = false;
        chatDuration = 0f;
    }

    public void Halt() => path.destination = transform.position;

    void Update()
    {
        currentState?.OnUpdate();
    }
}