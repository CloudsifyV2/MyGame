using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple first-person camera. Attach this to the Camera and set `player` to the player's transform.
/// Mouse controls look; player yaw is rotated with the camera so movement aligns with view.
/// </summary>
public class FirstPersonCamera : MonoBehaviour
{
    [Tooltip("Player transform to follow and rotate (only yaw will be applied)")]
    public Transform player;

    [Tooltip("Eye height above player's position")]
    public float eyeHeight = 1.6f;

    [Tooltip("Forward offset to move the camera outward (helps hide head)")]
    public float forwardOffset = 0.25f;

    [Tooltip("Mouse sensitivity multiplier")]
    public float mouseSensitivity = 0.15f;

    [Tooltip("Maximum up/down look angle in degrees")]
    public float maxPitch = 89f;

    private float yaw = 0f;
    private float pitch = 0f;

    private Vector2 lookInput;

    private bool syncedWithPlayer = false;

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        // Read mouse input
        if (Mouse.current != null)
            lookInput = Mouse.current.delta.ReadValue();
        else
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Sync initial rotation with player
        if (player != null && !syncedWithPlayer)
        {
            yaw = player.eulerAngles.y;
            pitch = transform.eulerAngles.x;

            syncedWithPlayer = true;
        }

        // Apply mouse look
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // Rotate player (yaw only) and position camera
        if (player != null)
        {
            player.rotation = Quaternion.Euler(0f, yaw, 0f);

            // NEW: forward offset added to help hide head
            transform.position =
                player.position +
                Vector3.up * eyeHeight +
                transform.forward * forwardOffset;
        }

        // Apply camera rotation (pitch + yaw)
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
