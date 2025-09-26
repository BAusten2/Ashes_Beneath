// Import libraries
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Player Dependencies
    [Header("Refs")]
    public CharacterController controller;
    public Transform cameraPivot;
    public LayerMask interactableMask;
    public LayerMask losBlockers;         
    public AntagonistAI[] enemies;           

    // Player Control (Using Unity Prebuilt Movement Packs)
    [Header("Input (Input System)")]
    public InputActionReference move;
    public InputActionReference look; 
    public InputActionReference jump; 
    public InputActionReference sprint;
    public InputActionReference interact;   

    //Player Speed/Physics
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7.0f;         
    public float gravity = -9.81f;
    public float jumpHeight = 1.1f;
    public float mouseSensitivity = 0.15f;
    public float interactDistance = 2.2f;

    // Footsteps/Audio System !DEPRECIATED!
    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;       
    public float stepIntervalWalk = 0.48f; 
    public float stepIntervalSprint = 0.36f;
    [Range(0f, 1f)] public float footstepLoudnessWalk = 0.45f;
    [Range(0f, 1f)] public float footstepLoudnessSprint = 0.9f;

    // Runtime
    Vector3 velocity;
    float pitch;
    float stepTimer;
    bool grounded;
    bool isHidden;
    Locker currentLocker;

    void Reset()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main) cameraPivot = Camera.main.transform;
    }

    // Auto IDs Character Controler and Antagonist. Focuses mouse.
    void Awake()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (enemies == null || enemies.Length == 0) enemies = FindObjectsOfType<AntagonistAI>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // On GameStart: Enable Action/Input Reference
    void OnEnable()
    {
        move?.action.Enable(); look?.action.Enable();
        jump?.action.Enable(); sprint?.action.Enable();
        interact?.action.Enable();
    }

    // On GameStop: Diaable Action/Input Reference
    void OnDisable()
    {
        move?.action.Disable(); look?.action.Disable();
        jump?.action.Disable(); sprint?.action.Disable();
        interact?.action.Disable();
    }

    // Standard update called every frame (tick?)
    void Update()
    {
        if (isHidden)
        {
            // Exit locker
            if (interact != null && interact.action.WasPressedThisFrame())
                TryExitLocker();
            return;
        }

        // Look
        if (look != null && cameraPivot)
        {
            Vector2 d = look.action.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(0f, d.x, 0f);
            pitch = Mathf.Clamp(pitch - d.y, -85f, 85f);
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        // Move
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        Vector2 mv = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 local = new Vector3(mv.x, 0f, mv.y);
        Vector3 world = transform.TransformDirection(local);
        bool wantsSprint = sprint && sprint.action.IsPressed();
        float speed = wantsSprint ? sprintSpeed : walkSpeed;

        controller.Move(world * speed * Time.deltaTime);

        // Jump
        if (grounded && jump != null && jump.action.WasPressedThisFrame())
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Footsteps !DEPRECIATED!
        float horizVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
        bool moving = horizVel > 0.2f && grounded;
        float stepInterval = wantsSprint ? stepIntervalSprint : stepIntervalWalk;
        if (moving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepTimer = 0f;
                PlayFootstep(wantsSprint);
            }
        }
        else stepTimer = 0f;

        // Interact (lockers/doors) !DEPRECIATED!
        if (interact != null && interact.action.WasPressedThisFrame())
            TryInteract();
    }

    // Audio Manager for Footsteps !DEPRECIATED!
    void PlayFootstep(bool sprinting)
    {
        if (footstepSource && footstepClips != null && footstepClips.Length > 0)
        {
            var clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.pitch = Random.Range(0.96f, 1.04f);
            footstepSource.PlayOneShot(clip);
        }
        // Volume Metric for Footsteps !DEPRECIATED!
        float loudness = sprinting ? footstepLoudnessSprint : footstepLoudnessWalk;
        foreach (var ai in enemies)
        {
            if (!ai) continue;
            ai.NotifyNoise(transform.position, loudness);
        }
    }

    // Determine Antagonist LoS when entering locker !DEPRECIATED!
    void TryInteract()
    {
        if (!cameraPivot) return;
        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask, QueryTriggerInteraction.Ignore))
        {
            var locker = hit.collider.GetComponentInParent<Locker>();
            if (locker != null)
            {
                // Determine LoS
                bool anyLoS = false;
                foreach (var ai in enemies)
                {
                    if (!ai) continue;
                    if (HasEnemyLineOfSight(ai.transform)) { anyLoS = true; break; }
                }

                if (locker.TryEnter(this))
                {
                    isHidden = true;
                    currentLocker = locker;
                    foreach (var ai in enemies)
                    {
                        if (!ai) continue;
                        ai.NotifyPlayerEnteredLocker(locker, anyLoS);
                    }
                }
                return;
            }
        }
    }


    void TryExitLocker()
    {
        if (currentLocker == null) return;
        if (currentLocker.TryExit(this))
        {
            isHidden = false;
            foreach (var ai in enemies)
            {
                if (!ai) continue;
                ai.NotifyPlayerExitedLocker();
            }
            currentLocker = null;
        }
    }
    
    // Determine whether Antgonist has LoS on Player
    bool HasEnemyLineOfSight(Transform enemy)
    {
        Vector3 origin = enemy.position + Vector3.up * 1.7f;
        Vector3 target = cameraPivot ? cameraPivot.position : transform.position + Vector3.up * 1.6f;
        Vector3 dir = target - origin;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == transform) return true;
            // Blocked if the first hit is on a los-blocking layer
            return false;
        }
        return true;
    }
}
