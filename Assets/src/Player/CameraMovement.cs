using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple first-person camera. Attach this to the Camera and set `player` to the player's transform.
/// Mouse controls look; player yaw is rotated with the camera so movement aligns with view (like Minecraft).
/// </summary>
public class FirstPersonCamera : MonoBehaviour
{
    [Tooltip("Player transform to follow and rotate (only yaw will be applied)")]
    public Transform player;

    [Tooltip("Eye height above player's position")]
    public float eyeHeight = 1.6f;

    [Tooltip("Mouse sensitivity multiplier")]
    public float mouseSensitivity = 0.15f;

    [Tooltip("Maximum up/down look angle in degrees")]
    public float maxPitch = 89f;

    private float yaw = 0f;
    private float pitch = 0f;

    private Vector2 lookInput;
    // ensure we sync initial camera yaw/pitch to player when player is assigned
    private bool syncedWithPlayer = false;

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current != null)
            lookInput = Mouse.current.delta.ReadValue();
        else
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // If the player was just assigned, sync yaw/pitch to avoid snapping the player
        if (player != null && !syncedWithPlayer)
        {
            // discard any accumulated raw mouse delta so the first frame doesn't produce a large jump
            if (Mouse.current != null)
                _ = Mouse.current.delta.ReadValue();

            yaw = player.eulerAngles.y;
            pitch = transform.eulerAngles.x;
            syncedWithPlayer = true;
        }

        // apply mouse look
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // rotate player (yaw only)
        if (player != null)
        {
            player.rotation = Quaternion.Euler(0f, yaw, 0f);
            // position camera at player's eye
            transform.position = player.position + Vector3.up * eyeHeight;
        }
        else
        {
            // if no player assigned, keep camera position unchanged
        }

        // apply camera pitch locally
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
