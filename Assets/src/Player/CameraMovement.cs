using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 2.0f;
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float cameraHeightOffset = 1.5f;


    private float x = 0.0f;
    private float y = -100.0f;

    private Vector2 lookInput;
    private float scrollInput;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        lookInput = Mouse.current.delta.ReadValue();
        scrollInput = Mouse.current.scroll.ReadValue().y;
    }

    void LateUpdate()
    {
        if (target)
        {
            x += lookInput.x * xSpeed * Time.deltaTime;
            y -= lookInput.y * ySpeed * Time.deltaTime;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            // Rotate player with camera (Y axis only)
            target.rotation = Quaternion.Euler(0, x, 0);

            distance -= scrollInput * zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 offset = new Vector3(0, cameraHeightOffset, 0);
            Vector3 position = rotation * negDistance + target.position + offset;

            transform.rotation = rotation;
            transform.position = position;
        }
    }


    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
