using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using DG.Tweening;

public class Blackboard
{
    public Vector3? lastSeenEnemy;
    public float lastSeenEnemyTimestamp;
    public Vector3? lastImpactPos;
    public float lastImpactPosTimestamp;
    public Vector3? lastImpactSrcDir;
    public Vector3? lastHeardPos;
    public float lastHeardPosTimestamp;
    public AIAgent allyToChat;
    public float chatDuration;
    public float alertness; 
    public float chatDesire = 0;
    public bool isRePositioning;
}

public enum StimulusType
{
    Impact, Gunshot, AlertnessNoise, SeeEnemy, LostSight, BeingHit
}

public readonly struct Stimulus
{
    public readonly StimulusType type;
    public readonly Vector3 position;
    public readonly float timeStamp;
    public readonly float ttl; //time to live

    public readonly bool IsValid => timeStamp > 0;
    public Stimulus(StimulusType type, Vector3 position, float timeStamp, float ttl)
    {
        this.type = type;
        this.position = position;
        this.timeStamp = timeStamp;
        this.ttl = ttl;
    }
}

public class PerceptionInbox
{
    readonly Queue<Stimulus> queue = new();
    public void Push(Stimulus s) => queue.Enqueue(s);
    public bool TryConsume(out Stimulus toConsume)
    {
        while(queue.Count > 0)
        {
            var s = queue.Dequeue();
            if(Time.time - s.timeStamp < s.ttl) {toConsume = s; return true;}
        }
        toConsume = default;
        return false;
    }
}


[DefaultExecutionOrder(0)]
public class AIAgent : MonoBehaviour
{
    AIPath path;

    public Transform player;
    public LayerMask obstacleMask;
    public float walkSpeed;
    public float chaseSpeed;
    public List<Transform> patrolWaypoints;
    public float viewAngle = 120.0f;
    public Transform muzzle;

    Vector3 destination;
    [HideInInspector] public bool isMoving;

    public bool isDebugMode = true;

    public void OnHit() => beingHit = true;
    [HideInInspector] public bool beingHit;

    public bool HasReachDestination()
    {
        if (Vector2.Distance(destination, transform.position) < 2f)
        {
            if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} has reach destination");
            isMoving = false;
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

    public IState observing;
    //sometimes, npc need to break states to spend some time staring at something
    public IState currentState;

    PerceptionInbox stimulusQueue;

    //Alert Behavior
    float alertnessDecaySpeed = 5f;
    public static event Action<float> UpdateSuspicous;
    public static event Action<string> UpdateState;

    //Observe Behavior
    float jitterMagnitude = 1f;

    //Chat Behavior
    HashSet<AIAgent> nearbyAllies;
    [SerializeField] float minChatDuration;
    [SerializeField] float maxChatDuration;
    [SerializeField] float chatDesireIncPerSec = 5f;

    [SerializeField] StateMonitor monitor;

    //Reinforce Behavior
    [SerializeField] ReinforceIcon reinforceIcon;
    [SerializeField] bool canCallReinforcement;
    [SerializeField] bool canReinforce;
    

    void Awake()
    {
        path = GetComponent<AIPath>();
        nearbyAllies = new();
    }

    void OnEnable()
    {
        AICommander.Instance.RequestReport += ReportBack;
        Weapon.Gunshot += ReactToGunshot;
    }
    void OnDisable()
    {
        AICommander.Instance.RequestReport -= ReportBack;
        Weapon.Gunshot -= ReactToGunshot;
    }

    void Start()
    {
        Initialize();
        EnqueueTransition(patrolling);
    }

    void Initialize()
    {
        InitStates();
        InitStatePriorityIndex();
        stimulusQueue = new();
        VFXManager.Instance.ProduceImpact += ReactToImpact;
        path.maxSpeed = walkSpeed;
    }

    void OnDestroy()
    {
        VFXManager.Instance.ProduceImpact -= ReactToImpact;
        raiseAlarmTween?.Kill();
    }

    void InitStates()
    {
        patrolling = new Patrolling(this);
        attacking = new Attacking(this);
        chasing = new Chasing(this);
        searching = new Searching(this);
        chatting = new Chatting(this);
        observing = new Observing(this);
    }

    void InitStatePriorityIndex()
    {
        statePriorityIndex = new()
        {
            { patrolling, 30},
            { attacking, 70},
            { chasing, 60},
            { searching, 50},
            { observing, 40},
            { chatting, 20 }
        };
    }

    public IEnumerator Observe(Vector3 point, float duration = 5f)
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is observing {point}");
        yield return StartCoroutine(LookAt(point)); //StartCoroutine 完成後才會繼續往下

        float elapsed = 0;
        while (elapsed < duration)
        {
            Vector3 nextView = point + (Vector3)UnityEngine.Random.insideUnitCircle * jitterMagnitude;
            yield return StartCoroutine(LookAt(nextView, 0.4f));

            float waitTime = UnityEngine.Random.Range(0.5f, 3.5f);
            yield return new WaitForSeconds(waitTime);

            elapsed += 0.2f + waitTime;
        }
    }

    public IEnumerator LookAt(Vector3 point, float duration = 0.3f)
    {
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

    public bool TryMoveTo(Vector3? pos, float speed = 0)
    {
        if (!pos.HasValue) return false;
        Vector3 targetPos = pos.Value;

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
            isMoving = true;
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

    public void CallReinforcement(Vector3 pos)
    {
        if(!canCallReinforcement) return;

        reinforceIcon.CallReinforcement();
        AICommander.Instance.CallReinforcement(this, pos);
    }

    void ReportBack(AIAgent sender)
    {
        if (sender == this || !canReinforce) return;
        AICommander.Instance.ReceiveReport(this, transform.position);
    }

    public void Reinforce(Vector3 pos)
    {
        blackboard.lastSeenEnemy = pos;
        blackboard.lastSeenEnemyTimestamp = Time.time;
        EnqueueTransition(chasing);
    }

    public void AddNearbyAllies(AIAgent agent)
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} add {agent.gameObject.name} to nearbyAllies");
        nearbyAllies.Add(agent);
    }

    public void RemoveNearbyAllies(AIAgent agent)
    {
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} remove {agent.gameObject.name} from nearbyAllies");
        nearbyAllies.Remove(agent);
    }

    public bool TryFindAllyToChat()
    {
        blackboard.chatDesire -= 10f;

        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is trying to find someone to chat");
        nearbyAllies.RemoveWhere(a => a == null || Vector3.Distance(transform.position, a.transform.position) > 10f);

        var allyList = nearbyAllies
            .Where(a => NoObstacleBetween(a.gameObject.transform.position))
            .ToList();

        if (isDebugMode) Debug.Log($"[AIAgent] Nearby Ally count: {allyList.Count}, chatDesire = {blackboard.chatDesire}");
        if (allyList.Count == 0) return false;
        int indexToPick = UnityEngine.Random.Range(0, allyList.Count);
        return TryStartChat(allyList[indexToPick]);
    }

    public bool TargetInSight(Vector3? targetPos)
    {
        if (!targetPos.HasValue) Debug.LogError("[AIAgent] TargetInSight accept only non null");
        return IsTargetInfront(targetPos) && NoObstacleBetween(targetPos);
    }

    bool IsTargetInfront(Vector3? targetPos)
    {
        if (!targetPos.HasValue) return false;

        //is target in viewAngle?
        Vector2 dirToPlayer = targetPos.Value - transform.position;
        float angle = Vector2.Angle(dirToPlayer, transform.up); //face up
        if (viewAngle / 2f < angle) return false;
        return true;
    }

    bool NoObstacleBetween(Vector3? targetPos)
    {
        //obstacle is those that blocks sight
        Vector2 dirToPlayer = targetPos.Value - transform.position;
        float distToTarget = dirToPlayer.magnitude;

        if (distToTarget > 20f) return false; //too far, can't see

        RaycastHit2D hit
        = Physics2D.Raycast(transform.position, dirToPlayer, distToTarget, obstacleMask);

        return hit.collider == null;
    }

    bool TryStartChat(AIAgent agent)
    {
        if (isMoving || agent.isMoving) return false; //移動中禁止交談
        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} is asking if {agent.gameObject.name} want to chat");
        blackboard.chatDuration = UnityEngine.Random.Range(minChatDuration, maxChatDuration);

        if (!agent.AcceptToChat(this)) return false;

        blackboard.allyToChat = agent;
        EnqueueTransition(chatting);
        return true;
    }

    public bool AcceptToChat(AIAgent ally)
    {
        if (currentState != patrolling || blackboard.chatDesire < 50)
        {
            if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} refuses to chat with {ally.gameObject.name}, desire = {blackboard.chatDesire}");
            return false;
        }

        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} accepts to chat with {ally.gameObject.name}");
        blackboard.chatDuration = ally.blackboard.chatDuration;

        if (isDebugMode) Debug.Log($"[AIAgent] {gameObject.name} knows the chat should last for {blackboard.chatDuration} sec");
        blackboard.allyToChat = ally;
        EnqueueTransition(chatting);
        return true;
    }

    public void SubscribeToPlayerNoiseSpeaker()
    {
        PlayerControl.ProduceNoise += ReactToPlayerNoise;
        if (isDebugMode) Debug.Log("[AIAgent] subscribe to noise event");
    }

    public void UnsubscribeToPlayerNoiseSpeaker()
    {
        PlayerControl.ProduceNoise -= ReactToPlayerNoise;
        if (isDebugMode) Debug.Log("[AIAgent] unsubscribe to noise event");
    }

    //Maximum receiving frequency is 0.1s (constrained by speaker produce's rate)
    void ReactToPlayerNoise(float volume)
    {
        blackboard.alertness = Mathf.Clamp(blackboard.alertness + volume, 0, 100);

        if (currentState == searching || currentState == observing && blackboard.alertness < 91) RaiseAlarm();
        if (blackboard.alertness > 90)
        {
            Stimulus stimulus = new(StimulusType.AlertnessNoise, player.position, Time.time, 5f);
            stimulusQueue.Push(stimulus);
        }
    }

    Tween raiseAlarmTween;
    void RaiseAlarm()
    {
        if (blackboard.alertness >= 95) return;
        if (raiseAlarmTween != null && raiseAlarmTween.IsPlaying()) return;

        raiseAlarmTween = DOTween.To(
            () => blackboard.alertness,
            value => blackboard.alertness = value,
            100,
            0.3f
        ).SetEase(Ease.OutQuad).SetAutoKill(true).SetLink(gameObject) ;
    }

    void ReactToEnemyInSight()
    {
        if(player == null || blackboard.isRePositioning) return;
        if (TargetInSight(player.transform.position))
        {
            Stimulus stimulus = new(StimulusType.SeeEnemy, player.position, Time.time, 5f);
            stimulusQueue.Push(stimulus);
            enemyInSight = true;
            if (blackboard.alertness < 91) RaiseAlarm();
        }
        else if (enemyInSight)
        {
            Stimulus stimulus = new(StimulusType.LostSight, player.position, Time.time, 5f);
            stimulusQueue.Push(stimulus);
            enemyInSight = false;
        }
    }

    void ReactToImpact(Vector3 pos, Vector3 source)
    {
        if(Vector3.Distance(transform.position, pos) > 15f) return;

        RaiseAlarm();
        Stimulus stimulus;

        if(Vector3.Distance(transform.position, pos) < 2f) //being hit
        {
            stimulus = new(StimulusType.BeingHit, player.position, Time.time, 5f);
            stimulusQueue.Push(stimulus); 
            return;
        }

        blackboard.lastImpactSrcDir = source; 
        stimulus = new(StimulusType.Impact, pos, Time.time, 5f);
        stimulusQueue.Push(stimulus);
    }

    void ReactToGunshot(Vector3 pos, float volume)
    {
        if (Vector3.Distance(transform.position, pos) > volume) return;

        //if the situation is being considered, don't react
        if (blackboard.lastHeardPos.HasValue
        && Vector3.Distance(pos, blackboard.lastHeardPos.Value) < 2
        && blackboard.lastHeardPosTimestamp - Time.time < 5) return;

        Stimulus stimulus = new(StimulusType.Gunshot, pos, Time.time, 5f);
        stimulusQueue.Push(stimulus);
        RaiseAlarm();
    }

    public void Halt() => path.destination = transform.position;

    Dictionary<IState, int> statePriorityIndex;
    IState pendingNext; int pendingPriority;
    public void EnqueueTransition(IState state, int priority = -1)
    {
        if (priority == -1)
        {
            statePriorityIndex.TryGetValue(state, out var defaultPriority);
            priority = defaultPriority;
        }
        
        if (pendingNext == null || pendingPriority < priority)
            {
                pendingNext = state; 
                pendingPriority = priority;
            }
    }

    bool enemyInSight;

    void Update()
    {
        ReactToEnemyInSight();

        //Process all the stimulus gathered in each frame, then enqueue corresponding reaction
        while (stimulusQueue.TryConsume(out var toConsume)) HandleStimulus(toConsume);
        //The highest priority reaction survives among all the others
        if (pendingNext != null)
        {
            TransitionTo(pendingNext); 
            pendingNext = null;
        }
        //We then exec the behavior
        currentState?.OnUpdate();

        AlertnessDecay();
        monitor.UpdateMeter(blackboard.alertness);

        if (currentState != patrolling || isMoving) return;
        if(blackboard.chatDesire < 100)
        {
            blackboard.chatDesire = Mathf.Clamp(
                blackboard.chatDesire +Time.deltaTime * chatDesireIncPerSec, 0, 100);
        }
        else TryFindAllyToChat();
    }

    void AlertnessDecay()
    {
        if (blackboard.alertness <= 0 || currentState == chasing || currentState == attacking) return;

        float decayAmount = Time.deltaTime * alertnessDecaySpeed;
        if (currentState == searching || currentState == observing) decayAmount /= 2;

        blackboard.alertness = Mathf.Clamp(blackboard.alertness - decayAmount, 0, 100);
    }

    public Blackboard blackboard = new();

    void HandleStimulus(Stimulus stimulus)
    {
        switch (stimulus.type)
        {
            case StimulusType.SeeEnemy:
                blackboard.lastSeenEnemy = stimulus.position;
                if(currentState != attacking) EnqueueTransition(attacking); 
                break;

            case StimulusType.AlertnessNoise:
                blackboard.lastHeardPos = stimulus.position;
                blackboard.lastHeardPosTimestamp = Time.time;
                if(currentState == attacking || currentState == chasing) break;

                if(currentState != searching) EnqueueTransition(searching);
                break;

            case StimulusType.Impact:
                blackboard.lastImpactPos = stimulus.position;
                blackboard.lastImpactPosTimestamp = Time.time;
                if(currentState == attacking) break;

                if(currentState != observing) EnqueueTransition(observing); 
                else 
                {
                    blackboard.lastSeenEnemy = player.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2f;
                    EnqueueTransition(chasing); 
                }
                break;
                
            case StimulusType.LostSight:
                blackboard.lastSeenEnemy = stimulus.position;
                blackboard.lastSeenEnemyTimestamp = Time.time;

                if(!blackboard.isRePositioning) EnqueueTransition(chasing); 
                break;

            case StimulusType.BeingHit:
                blackboard.lastSeenEnemy = stimulus.position;
                blackboard.lastSeenEnemyTimestamp = Time.time;
                if(currentState == attacking) break;

                if(!blackboard.isRePositioning) EnqueueTransition(chasing); 
                break;

            case StimulusType.Gunshot:
                blackboard.lastHeardPos = stimulus.position;
                blackboard.lastHeardPosTimestamp = Time.time;
                if(currentState == attacking) break;

                if(currentState == patrolling || currentState == chatting)
                {
                    EnqueueTransition(observing);
                    break;
                } else if(currentState == observing)
                {
                    blackboard.lastSeenEnemy = player.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2f;
                    EnqueueTransition(chasing); 
                }
                break;
        }
    }

    public void TransitionTo(IState state)
    {
        currentState?.OnExit();
        currentState = state;
        state.OnEnter();

        monitor.SetStateIcon(currentState.Name);
    }
}