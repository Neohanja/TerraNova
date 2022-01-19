using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmass : MonoBehaviour
{
    public static readonly int VertRows = 241;
    public static int MapSize { get { return VertRows - 1; } }

    public World worldEditor;
    public GameObject test;
    public bool autoUpdate;
    public Vector2Int testOffset;
    public int testSeed;
    [Header("Generation Rules")]
    public GenRules worldGen;

    // Start is called before the first frame update
    void Start()
    {
        //Remove, this is a debug item only.
        Destroy(gameObject);
    }

    public void GenerateMap()
    {
        MeshFilter testMesh = test.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        float[,] heightMap = BuildChunk.HeightMap(testOffset, VertRows, worldEditor.biomes, worldGen, testSeed);

        Vector3[] verts = new Vector3[VertRows * VertRows];
        Vector2[] uvMap = new Vector2[VertRows * VertRows];
        int[] tris = new int[MapSize * MapSize * 6];
        int tIndex = 0;

        for (int y = 0; y < VertRows; y++)
        {
            for (int x = 0; x < VertRows; x++)
            {
                int vIndex = y * VertRows + x;

                verts[vIndex] = new Vector3(x, heightMap[x, y] * worldGen.growth + worldGen.minHeight, y);
                uvMap[vIndex] = new Vector2(x / (float)VertRows, y / (float)VertRows);

                if (x < MapSize && y < MapSize)
                {
                    tris[tIndex] = vIndex;
                    tris[tIndex + 1] = vIndex + VertRows;
                    tris[tIndex + 2] = vIndex + VertRows + 1;

                    tris[tIndex + 3] = vIndex + VertRows + 1;
                    tris[tIndex + 4] = vIndex + 1;
                    tris[tIndex + 5] = vIndex;

                    tIndex += 6;
                }
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvMap;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        testMesh.sharedMesh = mesh;
    }
}
