using UnityEngine;
using System.Collections.Generic;

namespace MyGame.WorldManager
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();
        
        private static bool debuggedOnce = false; // Only debug first chunk

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        public void BuildChunkMesh(Chunk chunkData)
        {
            chunk = chunkData;

            // Clear previous mesh data
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            // Debug: Count block types
            int grassCount = 0, dirtCount = 0, stoneCount = 0, sandCount = 0;

            // Build the mesh by adding visible faces only
            for (int x = 0; x < WorldData.ChunkSize; x++)
            {
                for (int y = 0; y < WorldData.ChunkHeight; y++)
                {
                    for (int z = 0; z < WorldData.ChunkSize; z++)
                    {
                        BlockType block = chunk.blocks[x, y, z];
                        if (block == BlockType.Air) continue;

                        // Count blocks
                        if (block == BlockType.Grass) grassCount++;
                        else if (block == BlockType.Dirt) dirtCount++;
                        else if (block == BlockType.Stone) stoneCount++;
                        else if (block == BlockType.Sand) sandCount++;

                        AddBlockMesh(x, y, z, block);
                    }
                }
            }

            //Debug.Log($"Chunk {chunk.position}: Grass={grassCount}, Dirt={dirtCount}, Stone={stoneCount}, Sand={sandCount}");
            //Debug.Log($"Total vertices: {vertices.Count}, triangles: {triangles.Count/3}, UVs: {uvs.Count}");

            // Create the mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.Optimize();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            // Assign material
            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/BlockMaterial");
            }
        }

        private void AddBlockMesh(int x, int y, int z, BlockType block)
        {
            // Check each face and only add if it's exposed (next to air or chunk boundary)
            // Front face (Z+)
            if (IsAir(x, y, z + 1))
                AddFace(x, y, z, 0, block);

            // Back face (Z-)
            if (IsAir(x, y, z - 1))
                AddFace(x, y, z, 1, block);

            // Top face (Y+)
            if (IsAir(x, y + 1, z))
                AddFace(x, y, z, 2, block);

            // Bottom face (Y-)
            if (IsAir(x, y - 1, z))
                AddFace(x, y, z, 3, block);

            // Right face (X+)
            if (IsAir(x + 1, y, z))
                AddFace(x, y, z, 4, block);

            // Left face (X-)
            if (IsAir(x - 1, y, z))
                AddFace(x, y, z, 5, block);
        }

        private bool IsAir(int x, int y, int z)
        {
            // Out of bounds = treat as air (exposed face)
            if (x < 0 || x >= WorldData.ChunkSize ||
                y < 0 || y >= WorldData.ChunkHeight ||
                z < 0 || z >= WorldData.ChunkSize)
            {
                return true;
            }

            return chunk.blocks[x, y, z] == BlockType.Air;
        }

        private void AddFace(int x, int y, int z, int faceIndex, BlockType block)
        {
            int vertCount = vertices.Count;

            // Add vertices for this face
            Vector3[] faceVertices = GetFaceVertices(faceIndex, x, y, z);
            vertices.AddRange(faceVertices);

            // Add triangles (two triangles per face)
            triangles.Add(vertCount + 0);
            triangles.Add(vertCount + 1);
            triangles.Add(vertCount + 2);
            triangles.Add(vertCount + 2);
            triangles.Add(vertCount + 3);
            triangles.Add(vertCount + 0);

            // Add UVs
            Vector2[] faceUVs = BlockTextures.GetUVs(block, faceIndex);
            
            // Debug first few UVs to verify they're correct
            // if (vertCount < 12) // First 3 faces
            // {
            //     Debug.Log($"Block {block}, Face {faceIndex}: UV0={faceUVs[0]}, UV1={faceUVs[1]}, UV2={faceUVs[2]}, UV3={faceUVs[3]}");
            // }
            
            uvs.AddRange(faceUVs);
        }

        private Vector3[] GetFaceVertices(int faceIndex, int x, int y, int z)
        {
            // Each face has 4 vertices
            switch (faceIndex)
            {
                case 0: // Front (Z+)
                    return new Vector3[]
                    {
                        new Vector3(x, y, z + 1),
                        new Vector3(x + 1, y, z + 1),
                        new Vector3(x + 1, y + 1, z + 1),
                        new Vector3(x, y + 1, z + 1)
                    };

                case 1: // Back (Z-)
                    return new Vector3[]
                    {
                        new Vector3(x + 1, y, z),
                        new Vector3(x, y, z),
                        new Vector3(x, y + 1, z),
                        new Vector3(x + 1, y + 1, z)
                    };

                case 2: // Top (Y+)
                    return new Vector3[]
                    {
                        new Vector3(x, y + 1, z),
                        new Vector3(x, y + 1, z + 1),
                        new Vector3(x + 1, y + 1, z + 1),
                        new Vector3(x + 1, y + 1, z)
                    };

                case 3: // Bottom (Y-)
                    return new Vector3[]
                    {
                        new Vector3(x, y, z + 1),
                        new Vector3(x, y, z),
                        new Vector3(x + 1, y, z),
                        new Vector3(x + 1, y, z + 1)
                    };

                case 4: // Right (X+)
                    return new Vector3[]
                    {
                        new Vector3(x + 1, y, z + 1),
                        new Vector3(x + 1, y, z),
                        new Vector3(x + 1, y + 1, z),
                        new Vector3(x + 1, y + 1, z + 1)
                    };

                case 5: // Left (X-)
                    return new Vector3[]
                    {
                        new Vector3(x, y, z),
                        new Vector3(x, y, z + 1),
                        new Vector3(x, y + 1, z + 1),
                        new Vector3(x, y + 1, z)
                    };

                default:
                    return new Vector3[4];
            }
        }
    }
}