using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public int fontSize = 24;
    public Color fontColor = Color.white;

    private float deltaTime = 0.0f;

    void Update()
    {
        // Smooth deltaTime to avoid jitter
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = fontColor;

        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);

        Rect rect = new Rect(10, 10, w, h * 2 / 100);
        GUI.Label(rect, text, style);
    }
}
