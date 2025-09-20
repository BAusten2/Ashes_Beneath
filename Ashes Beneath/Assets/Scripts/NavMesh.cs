using UnityEngine;
using UnityEngine.AI;

public class AntagonistAI : MonoBehaviour
{
    public enum State { Wandering, Tracking, Hunting }

    [Header("Refs")]
    public Transform player;                // assign at runtime or in Inspector
    public NavMeshAgent agent;              // required
    public LayerMask losBlockers;           // layers that block line of sight (e.g., Default, Walls)

    [Header("Radii & Speeds")]
    public float wanderRadius = 20f;
    public float trackingRadius = 18f;      // proximity that triggers Tracking
    public float trackingHysteresis = 2f;   // prevents flicker: must exceed radius+hyst to drop back to Wandering
    public float hearingRadius = 25f;       // max distance for footstep alerts
    public float wanderSpeed = 2.0f;
    public float trackSpeed = 2.5f;
    public float huntSpeed = 4.5f;          // keep just below player's sprint speed

    [Header("Behaviour Timing")]
    public float newWanderPointEvery = 4f;
    public float hideForgetSeconds = 5f;    // time hidden before forgetting the player
    public float screamCooldown = 2f;       // avoid spamming

    [Header("Hunting")]
    public AudioSource screamSource;        // optional
    public AudioClip screamClip;

    // runtime
    public State state = State.Wandering;
    Vector3 lastKnownPlayerPos;
    float nextWanderPickAt;
    float hiddenSince = -1f;
    float lastScreamAt = -999f;
    bool sawPlayerEnterLocker = false;
    Locker lastSeenLocker = null;

    // Hooks from other systems
    public bool PlayerIsHidden { get; set; } = false;
    public Locker PlayerLocker { get; set; } = null;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!player && Camera.main) player = Camera.main.transform; // temp
    }

    void Update()
    {
        if (!agent || !player) return;

        // Global transition: Line-of-Sight always escalates to Hunting
        if (HasLineOfSight())
            SetState(State.Hunting);

        switch (state)
        {
            case State.Wandering: TickWandering(); break;
            case State.Tracking: TickTracking(); break;
            case State.Hunting: TickHunting(); break;
        }
    }

    // ===== States =====
    void TickWandering()
    {
        agent.speed = wanderSpeed;

        if (Time.time >= nextWanderPickAt || Reached(agent.destination))
        {
            agent.SetDestination(RandomPointOnNavmesh(transform.position, wanderRadius));
            nextWanderPickAt = Time.time + newWanderPointEvery;
        }

        // Enter Tracking when close enough
        if (DistanceToPlayer() <= trackingRadius)
            SetState(State.Tracking);
    }

    void TickTracking()
    {
        agent.speed = trackSpeed;

        // Hover around the player, meandering inward slowly
        Vector3 jitter = Random.insideUnitSphere * 2f; jitter.y = 0f;
        Vector3 target = player.position + (player.forward * -1f) + jitter; // drift behind/around player
        agent.SetDestination(target);
        lastKnownPlayerPos = player.position;

        // Player hidden? Start/maintain forget timer (only if not visible)
        if (PlayerIsHidden && !HasLineOfSight())
        {
            if (hiddenSince < 0f) hiddenSince = Time.time;
            if (Time.time - hiddenSince >= hideForgetSeconds)
                SetState(State.Wandering);
        }
        else hiddenSince = -1f;

        // Player escaped proximity?
        if (DistanceToPlayer() > (trackingRadius + trackingHysteresis))
            SetState(State.Wandering);
    }

    void TickHunting()
    {
        agent.speed = huntSpeed;

        // One-time scream on enter or after cooldown
        if (Time.time - lastScreamAt > screamCooldown)
        {
            if (screamSource && screamClip) screamSource.PlayOneShot(screamClip);
            lastScreamAt = Time.time;
        }

        // Chase: home in on player's current/last known position
        if (HasLineOfSight())
        {
            lastKnownPlayerPos = player.position;
            agent.SetDestination(player.position);
        }
        else
        {
            // keep going to last known, keeps pressure even if LoS breaks
            agent.SetDestination(lastKnownPlayerPos);
        }

        // Locker logic
        if (PlayerIsHidden)
        {
            if (sawPlayerEnterLocker && PlayerLocker) // saw them enter: attack
            {
                agent.SetDestination(PlayerLocker.attackPoint != null ?
                                     PlayerLocker.attackPoint.position : PlayerLocker.transform.position);
                if (Vector3.Distance(transform.position, PlayerLocker.transform.position) < 1.6f)
                {
                    PlayerLocker.Attack(); // define this to damage/end game
                    // after attack, choose: remain hunting or reset
                    SetState(State.Wandering);
                }
            }
            else if (!HasLineOfSight()) // lost LoS and player is hidden: forget after timer
            {
                if (hiddenSince < 0f) hiddenSince = Time.time;
                if (Time.time - hiddenSince >= hideForgetSeconds)
                    SetState(State.Wandering);
            }
        }
        else { hiddenSince = -1f; sawPlayerEnterLocker = false; lastSeenLocker = null; }

        // Outrun condition: if player gets far beyond trackingRadius, cool down
        if (DistanceToPlayer() > (trackingRadius + trackingHysteresis))
            SetState(State.Wandering);
    }

    void SetState(State s)
    {
        if (state == s) return;

        // on-exit
        if (state == State.Hunting) { /* could stop special FX, etc. */ }

        state = s;

        // on-enter
        if (state == State.Wandering)
        {
            hiddenSince = -1f;
            sawPlayerEnterLocker = false;
            lastSeenLocker = null;
            nextWanderPickAt = 0f; // force immediate pick
        }
        else if (state == State.Tracking)
        {
            hiddenSince = -1f;
        }
        else if (state == State.Hunting)
        {
            // ensure we immediately chase
            lastKnownPlayerPos = player.position;
        }
    }

    // ===== Events you can call from other scripts =====

    // Call this from your footstep system when the player makes noise.
    public void NotifyNoise(Vector3 pos, float loudness /*0..1*/)
    {
        if (loudness <= 0f) return;
        float d = Vector3.Distance(transform.position, pos);
        if (d <= hearingRadius)
        {
            lastKnownPlayerPos = pos;
            SetState(State.Hunting); // escalates to hunt on loud footstep
        }
    }

    // Call this when the player ENTERS a locker; pass true if the enemy had LoS at that moment.
    public void NotifyPlayerEnteredLocker(Locker locker, bool hadLineOfSight)
    {
        PlayerIsHidden = true;
        PlayerLocker = locker;
        if (hadLineOfSight)
        {
            sawPlayerEnterLocker = true;
            lastSeenLocker = locker;
            SetState(State.Hunting); // beeline and attack
        }
        else
        {
            // start forget timer from next Update (handled in states)
            hiddenSince = -1f;
        }
    }

    // Call this when the player EXITS a locker.
    public void NotifyPlayerExitedLocker()
    {
        PlayerIsHidden = false;
        PlayerLocker = null;
        sawPlayerEnterLocker = false;
        hiddenSince = -1f;
    }

    // ===== Helpers =====
    bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.7f;
        Vector3 target = player.position + Vector3.up * 1.6f;
        Vector3 dir = target - origin;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player) return true;
            // if the ray hits something else before the player and that thing is in losBlockers, it's blocked
            if ((losBlockers.value & (1 << hit.collider.gameObject.layer)) != 0) return false;
        }
        return false;
    }

    bool Reached(Vector3 p) => !agent.pathPending && agent.remainingDistance <= Mathf.Max(0.2f, agent.stoppingDistance);
    float DistanceToPlayer() => Vector3.Distance(transform.position, player.position);

    // sample a random reachable point on the NavMesh
    static Vector3 RandomPointOnNavmesh(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }
        return center; // fallback
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, trackingRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}

// Minimal Locker stub to illustrate API.
// Add a collider and optionally an "attackPoint" Transform where the enemy should stand to attack.
public class Locker : MonoBehaviour
{
    public Transform attackPoint;
    public void Attack()
    {
        Debug.Log("Locker attacked! Implement damage or game over here.");
    }
}
