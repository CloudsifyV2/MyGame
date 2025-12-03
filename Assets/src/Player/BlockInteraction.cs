using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.WorldManager;
using MyGame.Register;
using MyGame.Inventory;

public class BlockInteraction : MonoBehaviour
{
    public float reach = 6f;

    private Camera cam;
    private World world;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
            cam = Object.FindFirstObjectByType<Camera>();

        world = Object.FindFirstObjectByType<World>();
        if (world == null)
            Debug.LogError("BlockInteraction: No World found in scene.");
    }

    void Update()
    {
        if (cam == null || world == null) return;

        bool left = false;
        bool right = false;

        if (Mouse.current != null)
        {
            left = Mouse.current.leftButton.wasPressedThisFrame;
            right = Mouse.current.rightButton.wasPressedThisFrame;
        }
        else
        {
            left = Input.GetMouseButtonDown(0);
            right = Input.GetMouseButtonDown(1);
        }

        if (!left && !right) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, reach))
            return;

        var chunkRenderer = hit.collider.GetComponentInParent<ChunkRenderer>();
        if (chunkRenderer == null || chunkRenderer.chunk == null)
            return;

        Vector3 hitInside = hit.point - hit.normal * 0.01f;
        Vector3 local = hitInside - chunkRenderer.transform.position;

        int bx = Mathf.FloorToInt(local.x);
        int by = Mathf.FloorToInt(local.y);
        int bz = Mathf.FloorToInt(local.z);

        bx = Mathf.Clamp(bx, 0, WorldData.ChunkSize - 1);
        by = Mathf.Clamp(by, 0, WorldData.ChunkHeight - 1);
        bz = Mathf.Clamp(bz, 0, WorldData.ChunkSize - 1);

        Vector3 n = hit.normal;
        Vector3Int faceOffset;

        if (Mathf.Abs(n.x) > Mathf.Abs(n.y) && Mathf.Abs(n.x) > Mathf.Abs(n.z))
            faceOffset = new Vector3Int(n.x > 0 ? 1 : -1, 0, 0);
        else if (Mathf.Abs(n.y) > Mathf.Abs(n.x) && Mathf.Abs(n.y) > Mathf.Abs(n.z))
            faceOffset = new Vector3Int(0, n.y > 0 ? 1 : -1, 0);
        else
            faceOffset = new Vector3Int(0, 0, n.z > 0 ? 1 : -1);

        int worldX = Mathf.FloorToInt(chunkRenderer.transform.position.x) + bx;
        int worldY = by;
        int worldZ = Mathf.FloorToInt(chunkRenderer.transform.position.z) + bz;

        // BREAK BLOCK
        if (left)
        {
            BlockType oldBlock = world.GetBlockAt(worldX, worldY, worldZ);
            if (oldBlock == BlockType.Air) return;

            Item drop = BlockItemRegistry.GetItemForBlock(oldBlock);
            if (drop != null)
                Inventory.instance.Add(drop);

            world.SetBlockAt(worldX, worldY, worldZ, BlockType.Air);
        }

        // PLACE BLOCK FROM HOTBAR
        else if (right)
        {
            Item selected = HotbarSelector.instance != null ? HotbarSelector.instance.GetSelectedItem() : null;
            if (selected == null) return;

            BlockType placeType = BlockItemRegistry.GetBlockForItem(selected);
            if (placeType == BlockType.Air) return; // not a block item

            int placeX = worldX + faceOffset.x;
            int placeY = worldY + faceOffset.y;
            int placeZ = worldZ + faceOffset.z;

            if (placeY >= 0 && placeY < WorldData.ChunkHeight)
            {
                if (world.SetBlockAt(placeX, placeY, placeZ, placeType))
                {
                    Inventory.instance?.Remove(selected);
                }
            }
        }
    }
}
