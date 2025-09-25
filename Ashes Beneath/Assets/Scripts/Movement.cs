using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    [Header("Refs")]
    public CharacterController controller;
    public Transform cameraPivot; // assign your Camera transform

    [Header("Move")]
    public float moveSpeed = 4.5f;
    public float sprintSpeed = 7.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Look")]
    public float mouseSensitivity = 0.15f; // tweak to taste
    float pitch;

    [Header("Input (Input System)")]
    public InputActionReference move;   // Vector2
    public InputActionReference look;   // Vector2 (Pointer delta / Right stick)
    public InputActionReference jump;   // Button
    public InputActionReference sprint; // Button (held)

    Vector3 velocity;

    void Reset()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main) cameraPivot = Camera.main.transform;
    }
    void OnEnable() { move?.action.Enable(); look?.action.Enable(); jump?.action.Enable(); sprint?.action.Enable(); }
    void OnDisable() { move?.action.Disable(); look?.action.Disable(); jump?.action.Disable(); sprint?.action.Disable(); }

    void Update()
    {
        if (!controller) return;

        // Ground / gravity
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        // Movement
        Vector2 mv = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 local = new Vector3(mv.x, 0, mv.y);
        Vector3 world = transform.TransformDirection(local);
        float speed = (sprint && sprint.action.IsPressed()) ? sprintSpeed : moveSpeed;
        controller.Move(world * speed * Time.deltaTime);

        // Jump
        if (grounded && jump != null && jump.action.WasPressedThisFrame())
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Look
        if (cameraPivot && look != null)
        {
            Vector2 d = look.action.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(0f, d.x, 0f);
            pitch = Mathf.Clamp(pitch - d.y, -85f, 85f);
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
