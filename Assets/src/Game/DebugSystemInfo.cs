using UnityEngine;

namespace MyGame.Game
{
    [DefaultExecutionOrder(1000)]
    public class DebugSystemInfo : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.F1;
        public bool showOnStart = true;
        public Vector2 windowPos = new Vector2(10, 10);
        private bool visible;
        private float deltaTime = 0.0f;
        private GUIStyle boxStyle;
        private GUIStyle headerStyle;

        void Awake()
        {
            visible = showOnStart;
            // Do not access GUI.skin here (must be called from OnGUI). Create basic styles without skin.
            boxStyle = new GUIStyle();
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.padding = new RectOffset(8, 8, 8, 8);
            headerStyle = new GUIStyle();
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 14;
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                visible = !visible;

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!visible) return;

            int width = 360;
            int height = 265;

            Rect r = new Rect(windowPos.x, windowPos.y, width, height);
            GUILayout.BeginArea(r, GUI.skin.box);

            GUILayout.Label("System & World Debug", headerStyle);

            // FPS
            float fps = 1.0f / Mathf.Max(0.00001f, deltaTime);
            GUILayout.Label($"FPS: {fps:F1}");

            // Platform info
            GUILayout.Label($"OS: {SystemInfo.operatingSystem}");
            GUILayout.Label($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
            GUILayout.Label($"GPU: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB)");
            GUILayout.Label($"RAM (system): {SystemInfo.systemMemorySize} MB");

            // Memory from Unity's profiler (allocated)
            long allocated = 0;
            try
            {
                allocated = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            }
            catch { }
            GUILayout.Label($"Unity Allocated: {allocated / (1024 * 1024)} MB");

            // Screen
            GUILayout.Label($"Resolution: {Screen.width}x{Screen.height} ({Screen.currentResolution.refreshRate}hz)");

            // Player & world info
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 p = player.transform.position;
                GUILayout.Label($"Player Pos: X={p.x:F1} Y={p.y:F1} Z={p.z:F1}");

                // Biome at player
                var biome = MyGame.WorldManager.TerrainGenerator.GetBiome(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.z));
                GUILayout.Label($"Player Biome: {biome}");
            }
            else
            {
                GUILayout.Label("Player: <not found>");
            }

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
            else
            {
                GUILayout.Label("World: <not found>");
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close (F1)")) visible = false;
            if (GUILayout.Button("Center"))
            {
                // center the window on screen
                windowPos = new Vector2((Screen.width - width) / 2f, 10);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
