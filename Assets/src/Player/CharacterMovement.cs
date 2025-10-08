using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 moveInput;
    private bool isSprinting;
    private Vector3 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    void Update()
    {
        movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // Set Animator speed parameter
        if (movement.magnitude == 0)
            animator.SetFloat("Speed", 0f);         // Idle
        else if (isSprinting)
            animator.SetFloat("Speed", 0.6f);       // Run
        else
            animator.SetFloat("Speed", 0.3f);       // Walk
    }

    void FixedUpdate()
    {
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 targetPos = rb.position + movement * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        // Smooth rotation toward movement
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
