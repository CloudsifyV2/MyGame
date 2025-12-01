using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using MyGame.Player;

namespace MyGame.Game
{
    [DefaultExecutionOrder(1000)]
    public class DebugSystemInfo : MonoBehaviour
    {
        [Header("Keys")]
        public KeyCode toggleKey = KeyCode.F1;
        public KeyCode consoleKey = KeyCode.BackQuote; // ` key

        private bool visible = true;
        private bool consoleVisible = false;

        private float deltaTime = 0f;
        private Vector2 windowPos = new Vector2(10, 10);

        // Console
        private string input = "";
        private Vector2 consoleScroll;
        private List<string> consoleLog = new List<string>();

        private GUIStyle headerStyle;
        private GUIStyle consoleStyle;

        void Awake()
        {
            visible = true;

            headerStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };

            consoleStyle = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.white }
            };

            Application.logMessageReceived += OnLogMessage;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        // ----------------------------------------------------
        // NEW INPUT SYSTEM — SAFE KEYCODE → KEY MAPPER
        // ----------------------------------------------------
        private Key ConvertKeyCode(KeyCode code)
        {
            // Letters A–Z
            if (code >= KeyCode.A && code <= KeyCode.Z)
                return Key.A + (code - KeyCode.A);

            // Numbers 0–9
            if (code >= KeyCode.Alpha0 && code <= KeyCode.Alpha9)
                return Key.Digit0 + (code - KeyCode.Alpha0);

            // Function keys F1–F12
            if (code >= KeyCode.F1 && code <= KeyCode.F12)
                return Key.F1 + (code - KeyCode.F1);

            // Special keys
            return code switch
            {
                KeyCode.Space       => Key.Space,
                KeyCode.LeftShift   => Key.LeftShift,
                KeyCode.RightShift  => Key.RightShift,
                KeyCode.LeftControl => Key.LeftCtrl,
                KeyCode.RightControl=> Key.RightCtrl,
                KeyCode.LeftAlt     => Key.LeftAlt,
                KeyCode.RightAlt    => Key.RightAlt,
                KeyCode.Escape      => Key.Escape,
                KeyCode.Tab         => Key.Tab,
                KeyCode.Return      => Key.Enter,
                KeyCode.BackQuote   => Key.Backquote,

                _ => Key.None
            };
        }

        // ----------------------------------------------------
        // UPDATE
        // ----------------------------------------------------
        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            Key toggle = ConvertKeyCode(toggleKey);
            if (toggle != Key.None && Keyboard.current[toggle].wasPressedThisFrame)
                visible = !visible;

            Key consoleToggle = ConvertKeyCode(consoleKey);
            if (consoleToggle != Key.None && Keyboard.current[consoleToggle].wasPressedThisFrame)
                consoleVisible = !consoleVisible;
        }

        // ----------------------------------------------------
        // GUI
        // ----------------------------------------------------
        void OnGUI()
        {
            if (visible)
                DrawSystemWindow();

            if (consoleVisible)
                DrawConsoleWindow();
        }

        // ----------------------------------------------------
        // SYSTEM INFO WINDOW
        // ----------------------------------------------------
        private void DrawSystemWindow()
        {
            int width = 360;
            int height = 265;

            Rect r = new Rect(windowPos.x, windowPos.y, width, height);
            GUILayout.BeginArea(r, GUI.skin.box);

            GUILayout.Label("System & World Debug", headerStyle);

            float fps = 1.0f / Mathf.Max(0.00001f, deltaTime);
            GUILayout.Label($"FPS: {fps:F1}");

            GUILayout.Label($"OS: {SystemInfo.operatingSystem}");
            GUILayout.Label($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
            GUILayout.Label($"GPU: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB)");
            GUILayout.Label($"RAM (system): {SystemInfo.systemMemorySize} MB");

            long allocated = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            GUILayout.Label($"Unity Allocated: {allocated / (1024 * 1024)} MB");

            GUILayout.Label($"Resolution: {Screen.width}x{Screen.height} ({Screen.currentResolution.refreshRate}hz)");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 p = player.transform.position;
                GUILayout.Label($"Player Pos: X={p.x:F1} Y={p.y:F1} Z={p.z:F1}");

                var movement = player.GetComponent<CharacterMovement>();
                GUILayout.Label(movement != null 
                    ? $"Player Grounded: {movement.isGrounded}"
                    : "Player Grounded: <no CharacterMovement>");

                var biome = MyGame.WorldManager.TerrainGenerator.GetBiome(
                    Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.z));
                GUILayout.Label($"Player Biome: {biome}");
            }
            else GUILayout.Label("Player: <not found>");

            var world = Object.FindFirstObjectByType<MyGame.WorldManager.World>();
            if (world != null)
            {
                GUILayout.Label($"Loaded Chunks: {world.LoadedChunkCount}");
                GUILayout.Label($"Build Queue: {world.BuildQueueLength}");
                GUILayout.Label($"Queued Set: {world.QueuedSetCount}");
                GUILayout.Label($"Render Distance: {world.renderDistance}");
                GUILayout.Label($"Initial Load Radius: {world.initialLoadRadius}");
                GUILayout.Label($"Chunks/Frame: {world.chunksPerFrame}");
                GUILayout.Label($"Spawn Trees: {world.spawnTrees}");
            }
            else GUILayout.Label("World: <not found>");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close (F1)"))
                visible = false;

            GUILayout.EndArea();
        }

        // ----------------------------------------------------
        // CONSOLE WINDOW
        // ----------------------------------------------------
        private void DrawConsoleWindow()
        {
            int width = 600;
            int height = 300;

            Rect r = new Rect(10, Screen.height - height - 10, width, height);
            GUILayout.BeginArea(r, GUI.skin.box);

            GUILayout.Label("Debug Console", headerStyle);

            // Scrollable log
            consoleScroll = GUILayout.BeginScrollView(consoleScroll, GUILayout.Height(220));
            foreach (string line in consoleLog)
                GUILayout.Label(line, consoleStyle);
            GUILayout.EndScrollView();

            // Input bar
            GUILayout.BeginHorizontal();
            input = GUILayout.TextField(input, GUILayout.Height(25));
            if (GUILayout.Button("Run", GUILayout.Width(60)))
            {
                ExecuteCommand(input);
                input = "";
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        // ----------------------------------------------------
        // COMMAND EXECUTION
        // ----------------------------------------------------
        private void ExecuteCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
                return;

            consoleLog.Add($"> {cmd}");

            if (cmd == "clear")
            {
                consoleLog.Clear();
                return;
            }

            if (cmd == "log")
            {
                consoleLog.Add("Unity Console Dump:");
                return;
            }

            // Example: print PlayerData.Instance.gameMode.modeName
            if (cmd.StartsWith("print "))
            {
                string expression = cmd.Substring(6).Trim();

                if (expression == "PlayerData.Instance.gameMode.modeName")
                {
                    if (PlayerData.Instance == null)
                        consoleLog.Add("PlayerData.Instance = NULL");
                    else if (PlayerData.Instance.gameMode == null)
                        consoleLog.Add("PlayerData.Instance.gameMode = NULL");
                    else
                        consoleLog.Add("Gamemode: " + PlayerData.Instance.gameMode.modeName);

                    return;
                }

                consoleLog.Add("Unknown variable: " + expression);
                return;
            }

            consoleLog.Add("Unknown command.");
        }

        // ----------------------------------------------------
        // UNTIY LOG FORWARDING
        // ----------------------------------------------------
        private void OnLogMessage(string condition, string stack, LogType type)
        {
            consoleLog.Add($"[{type}] {condition}");
            consoleScroll.y = float.MaxValue;
        }
    }
}
