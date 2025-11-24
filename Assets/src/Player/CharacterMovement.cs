using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayerMask = 1;

    // Animator grounding helpers
    [SerializeField] private float groundedDelay = 0.12f;              // brief delay after jump before animator treats us as grounded
    [SerializeField] private float groundedVelocityThreshold = 0.2f;   // consider grounded only when vertical speed is small
    private float lastJumpTime = -10f;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 moveInput;
    private bool isSprinting;
    private bool jumpInput;
    private Vector3 movement;
    private bool isGrounded;

    // New: local tracking for Jump trigger (Animator triggers are not queryable)
    private bool lastLocalJumpTriggered = false;
    private float lastLocalJumpTime = 0f;
    private float localJumpDisplayDuration = 1f; // seconds to report the local trigger
    private bool prevAnimatorGrounded = true; // track previous animator-grounded state to detect landing

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Enable gravity on rigidbody
        rb.useGravity = true;

        // Try to find Animator on self or children
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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
        jumpInput = value.isPressed;
    }

    void Update()
    {
        // Ground check using raycast
       isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayerMask);
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Camera-relative movement
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Try to find camera if Camera.main is null
            Camera cam = Camera.main;
            if (cam == null)
                cam = Object.FindFirstObjectByType<Camera>();

            if (cam != null)
            {
                Vector3 camForward = cam.transform.forward;
                Vector3 camRight = cam.transform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                movement = (camRight * moveInput.x + camForward * moveInput.y).normalized;
            }
            else
            {
                // Fallback to world-space movement if no camera found
                movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            }
        }
        else
        {
            movement = Vector3.zero;
        }

        // Handle jumping
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            if (animator != null)
            {
                animator.SetTrigger("Jump"); // Use trigger instead of bool
                lastJumpTime = Time.time;    // track when jump started
            }

            jumpInput = false; // Consume jump input
        }

        // Animator parameters (null-safe)
        if (animator != null)
        {
            float speedParam = 0f;
            if (movement.magnitude > 0.1f)
                speedParam = isSprinting ? 0.6f : 0.3f;

            animator.SetFloat("Speed", speedParam);

            // Compute a safer "animator grounded" value so the animator doesn't flip to grounded
            // the instant we briefly touch ground during landing. Require small vertical velocity
            // or a short delay since the last jump.
            bool animatorGrounded = false;
            float timeSinceJump = Time.time - lastJumpTime;
            float verticalSpeed = rb != null ? rb.linearVelocity.y : 0f;
            if (isGrounded && (Mathf.Abs(verticalSpeed) < groundedVelocityThreshold || timeSinceJump > groundedDelay))
                animatorGrounded = true;
            
            animator.SetBool("isGrounded", animatorGrounded);
            //animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

            // Detect landing (was not grounded for animator previously, now is grounded)
            if (!prevAnimatorGrounded && animatorGrounded)
            {
                // small safety: only reset if we've jumped recently (avoid clearing unrelated triggers)
                if (Time.time - lastJumpTime <= 2f)
                {
                    animator.ResetTrigger("Jump");
                    lastLocalJumpTriggered = false;
                    Debug.Log("Landing detected - Reset Jump trigger");
                }
            }

            // store previous animator-grounded state for next frame
            prevAnimatorGrounded = animatorGrounded;
        }
    }

    void FixedUpdate()
    {
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Only move horizontally, let rigidbody handle vertical physics
        Vector3 horizontalMovement = movement * speed * Time.fixedDeltaTime;
        Vector3 targetPosition = rb.position + horizontalMovement;
        
        // Preserve Y position to let gravity work
        targetPosition.y = rb.position.y;
        
        rb.MovePosition(targetPosition);
    }

    // Helper: check for a parameter with a specific type and name
    private bool AnimatorHasParameter(string name, AnimatorControllerParameterType type)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
        {
            if (p.name == name && p.type == type) return true;
        }
        return false;
    }
}
