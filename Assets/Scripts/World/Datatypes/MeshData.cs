using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    Mesh chunkMesh;
    
    List<Vector3> verts;
    List<int> terraTris;
    List<int> liquidTris;
    List<Vector2> uvMap;
    int vertCount;

    public MeshData(Mesh attachMesh)
    {
        chunkMesh = attachMesh;
        verts = new List<Vector3>();
        terraTris = new List<int>();
        liquidTris = new List<int>();
        uvMap = new List<Vector2>();
        vertCount = 0;
    }

    public void AddTerraFace(Vector3 point, Face face, int textureID)
    {
        if (face == null || face.skipDraw) return;

        float x = textureID % TextureAtlas.GameTextures.tRows;
        float y = textureID / TextureAtlas.GameTextures.tRows;

        x *= TextureAtlas.GameTextures.tSize;
        y *= TextureAtlas.GameTextures.tSize;

        for (int v = 0; v < face.vertices.Length; ++v)
        {
            verts.Add(point + face.vertices[v]);
            uvMap.Add(new Vector2(x, y) + face.uvMap[v] * TextureAtlas.GameTextures.tSize);
        }

        for (int t = 0; t < face.trianges.Length; ++t)
        {
            terraTris.Add(vertCount + face.trianges[t]);
        }

        vertCount += face.vertices.Length;
    }

    public void AddLiquidFace(Vector3 point, Face face)
    {
        if (face == null || face.skipDraw) return;

        for (int v = 0; v < face.vertices.Length; ++v)
        {
            verts.Add(point + face.vertices[v]);
            uvMap.Add(face.uvMap[v]);
        }

        for (int t = 0; t < face.trianges.Length; ++t)
        {
            liquidTris.Add(vertCount + face.trianges[t]);
        }

        vertCount += face.vertices.Length;
    }

    public void ApplyMesh()
    {
        chunkMesh.Clear();

        chunkMesh.subMeshCount = 2;
        chunkMesh.vertices = verts.ToArray();
        chunkMesh.uv = uvMap.ToArray();
        chunkMesh.SetTriangles(terraTris.ToArray(), 0);
        chunkMesh.SetTriangles(liquidTris.ToArray(), 1);
        chunkMesh.RecalculateNormals();

        ClearMeshData();
    }

    public void ClearMeshData()
    {
        verts.Clear();
        terraTris.Clear();
        liquidTris.Clear();
        uvMap.Clear();
        vertCount = 0;
    }
}
