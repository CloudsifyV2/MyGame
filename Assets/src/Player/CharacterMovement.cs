using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;

    // Increased to avoid missed jumps
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayerMask = 1;

    [SerializeField] private float groundedDelay = 0.12f;
    [SerializeField] private float groundedVelocityThreshold = 0.2f;

    private Rigidbody rb;
    private Animator animator;
    private Collider col;

    private Vector2 moveInput;
    private bool isSprinting;
    private bool jumpPressed;       // NEW: stored jump input for FixedUpdate
    public bool isGrounded;
    private Vector3 movement;

    private float lastJumpTime = -10f;
    private bool prevAnimatorGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Prevent rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // cache collider for ground checks (falls back if none found)
        col = GetComponent<Collider>();
        if (col == null) col = GetComponentInChildren<Collider>();

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            jumpPressed = true;  // store it until FixedUpdate consumes it
    }

    void Update()
    {
        // Camera-relative movement direction
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Camera cam = Camera.main;
            if (cam == null) cam = Object.FindFirstObjectByType<Camera>();

            if (cam != null)
            {
                Vector3 forward = cam.transform.forward; forward.y = 0;
                Vector3 right = cam.transform.right;   right.y = 0;
                forward.Normalize(); right.Normalize();

                movement = (forward * moveInput.y + right * moveInput.x).normalized;
            }
        }
        else movement = Vector3.zero;

        // Animator-only logic that does NOT affect jumping physics
        if (animator != null)
        {
            float speedParam = movement.magnitude > 0.1f ?
                (isSprinting ? 0.6f : 0.3f) : 0f;
            animator.SetFloat("Speed", speedParam);
        }
    }

    void FixedUpdate()
    {
        // Ground check: use a SphereCast (more robust for thin/angled faces from chunk generator)
        float sphereRadius = 0.25f;
        if (col != null)
        {
            // use the smaller horizontal extent as base radius
            sphereRadius = Mathf.Max(0.12f, Mathf.Min(col.bounds.extents.x, col.bounds.extents.z) * 0.6f);
        }
        // start a little above the transform to avoid missing short geometry gaps
        Vector3 sphereOrigin = transform.position + Vector3.up * 0.1f;
        float castDistance = groundCheckDistance + 0.12f;
        isGrounded = Physics.SphereCast(sphereOrigin, sphereRadius, Vector3.down, out RaycastHit groundHit, castDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

        // Handle jump with ZERO delay
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
                lastJumpTime = Time.time;
            }

            jumpPressed = false; // consumed
        }

        // Movement
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = movement * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + new Vector3(move.x, 0f, move.z));

        // Animator grounding (separate from physics)
        if (animator != null)
        {
            float timeSinceJump = Time.time - lastJumpTime;
            float verticalSpeed = rb.linearVelocity.y;

            bool animatorGrounded = false;
            if (isGrounded &&
                (Mathf.Abs(verticalSpeed) < groundedVelocityThreshold || timeSinceJump > groundedDelay))
                animatorGrounded = true;

            animator.SetBool("isGrounded", animatorGrounded);

            if (!prevAnimatorGrounded && animatorGrounded)
            {
                if (Time.time - lastJumpTime <= 2f)
                    animator.ResetTrigger("Jump");
            }

            prevAnimatorGrounded = animatorGrounded;
        }
    }
}
