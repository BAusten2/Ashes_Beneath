using UnityEngine;
using UnityEngine.AI;

public class AntagonistAI : MonoBehaviour
{
    public enum State { Wandering, Tracking, Hunting }

    [Header("Refs")]
    public Transform player;
    public NavMeshAgent agent;
    public LayerMask losBlockers = ~0;
    public Animator animator;
    public AudioSource screamSource;
    public AudioClip screamClip;

    [Header("Radii / Speeds")]
    public float wanderRadius = 20f;
    public float trackingRadius = 18f;
    public float trackingHysteresis = 2f;
    public float hearingRadius = 25f;
    public float wanderSpeed = 2.0f;
    public float trackSpeed = 2.5f;
    public float huntSpeed = 4.6f;

    [Header("Vision")]
    public bool useFOV = false;
    [Range(1f, 360f)] public float fovAngle = 120f;
    public float eyeHeight = 1.7f;
    public float targetHeight = 1.6f;

    [Header("Timers")]
    public float newWanderPointEvery = 4f;
    public float hideForgetSeconds = 5f;
    public float screamCooldown = 200f;

    // runtime
    public State state = State.Wandering;
    Vector3 lastKnownPlayerPos;
    float nextWanderPickAt;
    float hiddenSince = -1f;
    float lastScreamAt = -999f;
    bool sawPlayerEnterLocker = false;
    Locker trackedLocker = null;
    bool _hasScreamedThisHunt = false;

    // player-hidden status
    public bool PlayerIsHidden { get; private set; }
    public Locker PlayerLocker { get; private set; }

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!player && Camera.main) player = Camera.main.transform;
    }

    void Update()
    {
        if (!agent || !player) return;

        // global escalation
        if (HasLineOfSight()) SetState(State.Hunting);

        switch (state)
        {
            case State.Wandering: TickWandering(); break;
            case State.Tracking: TickTracking(); break;
            case State.Hunting: TickHunting(); break;
        }

        if (animator)
        {
            animator.SetInteger("State", (int)state);
            animator.SetFloat("Speed", agent ? agent.velocity.magnitude : 0f);
        }
    }

    // ---- states ----
    void TickWandering()
    {
        agent.speed = wanderSpeed;

        if (Time.time >= nextWanderPickAt || Reached(agent.destination))
        {
            agent.SetDestination(RandomPointOnNavmesh(transform.position, wanderRadius));
            nextWanderPickAt = Time.time + newWanderPointEvery;
        }

        if (DistanceToPlayer() <= trackingRadius)
            SetState(State.Tracking);
    }

    void TickTracking()
    {
        agent.speed = trackSpeed;

        Vector3 jitter = Random.insideUnitSphere * 2f; jitter.y = 0f;
        Vector3 target = player.position + jitter;
        agent.SetDestination(target);
        lastKnownPlayerPos = player.position;

        if (PlayerIsHidden && !HasLineOfSight())
        {
            if (hiddenSince < 0f) hiddenSince = Time.time;
            if (Time.time - hiddenSince >= hideForgetSeconds)
                SetState(State.Wandering);
        }
        else hiddenSince = -1f;

        if (DistanceToPlayer() > trackingRadius + trackingHysteresis)
            SetState(State.Wandering);
    }

    void TickHunting()
    {
        agent.speed = huntSpeed;

        if (Time.time - lastScreamAt > screamCooldown)
        {
            if (screamSource && screamClip) screamSource.PlayOneShot(screamClip);
            lastScreamAt = Time.time;
        }

        if (HasLineOfSight())
        {
            lastKnownPlayerPos = player.position;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(lastKnownPlayerPos);
        }

        if (PlayerIsHidden)
        {
            if (sawPlayerEnterLocker && trackedLocker)
            {
                Vector3 pt = trackedLocker.attackPoint ?
                             trackedLocker.attackPoint.position : trackedLocker.transform.position;
                agent.SetDestination(pt);

                if (Vector3.Distance(transform.position, trackedLocker.transform.position) < 1.6f)
                {
                    trackedLocker.Attack(); 
                    SetState(State.Wandering);
                }
            }
            else if (!HasLineOfSight())
            {
                if (hiddenSince < 0f) hiddenSince = Time.time;
                if (Time.time - hiddenSince >= hideForgetSeconds)
                    SetState(State.Wandering);
            }
        }
        else
        {
            hiddenSince = -1f;
            sawPlayerEnterLocker = false;
            trackedLocker = null;
        }

        if (DistanceToPlayer() > trackingRadius + trackingHysteresis)
            SetState(State.Wandering);
    }

    void SetState(State s)
    {
        if (state == s) return;
        var prev = state;
        state = s;

        if (state == State.Hunting)
        {
            lastKnownPlayerPos = player.position;

            if (animator && !_hasScreamedThisHunt)
            {
                animator.ResetTrigger("Scream");
                animator.SetTrigger("Scream");
                _hasScreamedThisHunt = true;
            }
        }
        else
        {
            _hasScreamedThisHunt = false;
        }

        if (state == State.Wandering)
        {
            hiddenSince = -1f;
            sawPlayerEnterLocker = false;
            trackedLocker = null;
            nextWanderPickAt = 0f;
        }
    }

    // ---- events from other systems ----
    public void NotifyNoise(Vector3 pos, float loudness) // 0..1
    {
        float d = Vector3.Distance(transform.position, pos);
        if (d > hearingRadius) return;

        float norm = Mathf.Clamp01(1f - (d / hearingRadius));
        if (norm * loudness >= 0.35f)
        {
            lastKnownPlayerPos = pos;
            SetState(State.Hunting);
        }
    }

    public void NotifyPlayerEnteredLocker(Locker locker, bool hadLoS)
    {
        PlayerIsHidden = true;
        PlayerLocker = locker;
        if (hadLoS)
        {
            sawPlayerEnterLocker = true;
            trackedLocker = locker;
            SetState(State.Hunting);
        }
        hiddenSince = -1f;
    }

    public void NotifyPlayerExitedLocker()
    {
        PlayerIsHidden = false;
        PlayerLocker = null;
        sawPlayerEnterLocker = false;
        trackedLocker = null;
        hiddenSince = -1f;
    }

    // ---- helpers ----
    bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * targetHeight;
        Vector3 dir = target - origin;
        float dist = dir.magnitude;

        if (useFOV)
        {
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > fovAngle * 0.5f) return false;
        }

        // Raycast that only considers blockers and the player
        int mask = losBlockers | (1 << player.gameObject.layer);
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
            return hit.transform == player;

        return true; // nothing hit
    }

    bool Reached(Vector3 p) => !agent.pathPending && agent.remainingDistance <= Mathf.Max(0.2f, agent.stoppingDistance);
    float DistanceToPlayer() => Vector3.Distance(transform.position, player.position);

    static Vector3 RandomPointOnNavmesh(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = center + Random.insideUnitSphere * radius; candidate.y = center.y;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }
        return center;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, trackingRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
