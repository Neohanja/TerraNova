using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static readonly int ChunkSize = 32;
    public static readonly int ChunkHeight = 96;

    GameObject chunkObj;
    MeshFilter chunkMesh;
    MeshRenderer chunkRender;
    MeshData chunkMeshData;

    Vector2Int chunkCord;
    Vector2 chunkPos;
    byte[,,] chunkData;

    Chunk northChunk;
    Chunk eastChunk;
    Chunk southChunk;
    Chunk westChunk;

    public Chunk(Vector2Int point)
    {
        chunkObj = new GameObject("Chunk");
        chunkMesh = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();

        chunkRender.materials = World.WorldMap.worldMats;
        chunkMeshData = new MeshData(chunkMesh.mesh);

        chunkCord = point;
        chunkPos = point * ChunkSize;
        chunkObj.transform.position = new Vector3(chunkPos.x, 0, chunkPos.y);        
        chunkObj.transform.SetParent(World.WorldMap.transform);

        BuildChunkData(World.WorldMap.biomes, World.WorldMap.worldGen, World.WorldMap.worldSeed);
        BuildChunkMesh();
    }

    public void BuildChunkMesh()
    {
        Shapes shape = World.WorldMap.shapes[0];
        for (int x = 0; x < ChunkSize; ++x)
        {
            for (int y = 0; y < ChunkHeight; ++y)
            {
                for (int z = 0; z < ChunkSize; ++z)
                {
                    Vector3Int curPoint = new Vector3Int(x, y, z);
                    TileData curTile = GetTile(curPoint);

                    if (curTile != null && !curTile.skipDraw)
                    {
                        for(int face = 0; face < 6; ++face)
                        {
                            TileData checkTile = GetTile(curPoint + TileDirection[face]);

                            if(checkTile == null || checkTile.skipDraw || curTile.liquid != checkTile.liquid)
                            {
                                if (curTile.liquid)
                                {
                                    chunkMeshData.AddLiquidFace(curPoint, shape.GetFace(face));
                                }
                                else
                                {
                                    chunkMeshData.AddTerraFace(curPoint, shape.GetFace(face), curTile.GetTexture(face));                                    
                                }
                            }
                        }
                    }
                }
            }
        }

        chunkMeshData.ApplyMesh();
    }

    public void BuildChunkData(Biome[] biomes, GenRules noiseGen, int seed)
    {
        chunkData = new byte[ChunkSize, ChunkHeight, ChunkSize];
        float[,] heightMap = BuildChunk.HeightMap(chunkCord * ChunkSize, ChunkSize, biomes, noiseGen, seed);
        Vector2[,] biomeMap = BuildChunk.BiomeMap(chunkCord * ChunkSize, ChunkSize, biomes, noiseGen.vScale, seed);

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                int tHeight = MathFun.Round(heightMap[x, z] * noiseGen.growth) + noiseGen.minHeight;
                int biomeIndex = MathFun.Floor(biomeMap[x, z].y);

                for (int y = 0; y < ChunkHeight; y++)
                {
                    byte useTile = 0;

                    if (y == 0)
                    {
                        useTile = 1;
                    }
                    else if (y < noiseGen.seaLevel && y > tHeight)
                    {
                        // Water
                        useTile = 4;
                    }
                    else if (y < tHeight - biomes[biomeIndex].secondaryDepth)
                    {
                        useTile = 1;
                    }
                    else if (y < tHeight)
                    {
                        useTile = biomes[biomeIndex].secondaryTile;
                    }
                    else if (y == tHeight)
                    {
                        useTile = biomes[biomeIndex].primaryTile;
                    }
                    chunkData[x, y, z] = useTile;
                }
            }
        }
    }

    public void AttachChunk(ChunkDirection direction, Chunk chunk)
    {
        switch(direction)
        {
            case ChunkDirection.North:
                if (northChunk == chunk)
                    return;
                northChunk = chunk;
                chunk.AttachChunk(ChunkDirection.South, this);
                break;
            case ChunkDirection.East:
                if (eastChunk == chunk)
                    return;
                eastChunk = chunk;
                chunk.AttachChunk(ChunkDirection.West, this);
                break;
            case ChunkDirection.South:
                if (southChunk == chunk)
                    return;
                southChunk = chunk;
                chunk.AttachChunk(ChunkDirection.North, this);
                break;
            case ChunkDirection.West:
                if (westChunk == chunk)
                    return;
                westChunk = chunk;
                chunk.AttachChunk(ChunkDirection.East, this);
                break;
        }

        BuildChunkMesh();
    }

    public TileData GetTile(Vector3Int point)
    {
        if (point.y < 0 || point.y >= ChunkHeight) return null;

        if (point.x < 0)
        {
            if(westChunk == null) return null;
            else return westChunk.GetTile(new Vector3Int(point.x + ChunkSize, point.y, point.z));
        }
        if (point.x >= ChunkSize)
        {
            if (eastChunk == null) return null;
            else return eastChunk.GetTile(new Vector3Int(point.x - ChunkSize, point.y, point.z));
        }

        if (point.z < 0)
        {
            if (southChunk == null) return null;
            else return southChunk.GetTile(new Vector3Int(point.x, point.y, point.z + ChunkSize));
        }
        if (point.z >= ChunkSize)
        {
            if (northChunk == null) return null;
            else return northChunk.GetTile(new Vector3Int(point.x, point.y, point.z - ChunkSize));
        }

        return World.WorldMap.tiles[chunkData[point.x, point.y, point.z]];
    }

    public static readonly Vector3Int[] TileDirection = new Vector3Int[]
    {
        new Vector3Int( 0,  1,  0),
        new Vector3Int( 0, -1,  0),
        new Vector3Int( 0,  0,  1),
        new Vector3Int( 1,  0,  0),
        new Vector3Int( 0,  0, -1),
        new Vector3Int(-1,  0,  0)
    };

    public enum ChunkDirection
    {
        North,
        East,
        South,
        West
    }
}
