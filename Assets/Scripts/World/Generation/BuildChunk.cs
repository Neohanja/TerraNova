using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildChunk
{
    public static Vector2[,] VornoiModMap(Vector2Int chunkPos, int size, Biome[] biomes, float vScale, int seed)
    {
        Vector2[,] falloffMap = new Vector2[size, size];

        // (0, 0); (0, 1); (1, 0); (1, 1)
        float[] corners = new float[4];

        for (int cX = 0; cX < 2; cX++)
        {
            for (int cY = 0; cY < 2; cY++)
            {
                float sampleX = (chunkPos.x + cX * size) / vScale;
                float sampleY = (chunkPos.y + cY * size) / vScale;
                int bIndex = MathFun.Floor(Noise.VornoiNoise(new Vector2(sampleX, sampleY), biomes.Length, seed).y);

                corners[cX + cY * 2] = biomes[bIndex].biomeHeight;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float sampleX = (chunkPos.x + x) / vScale;
                float sampleY = (chunkPos.y + y) / vScale;
                int mainInfluence = MathFun.Floor(Noise.VornoiNoise(new Vector2(sampleX, sampleY), biomes.Length, seed).y);

                float cX = MathFun.Curve(x / (float)size);
                float cY = MathFun.Curve(y / (float)size);
                float x1 = MathFun.Lerp(corners[0], corners[1], cX);
                float x2 = MathFun.Lerp(corners[2], corners[3], cX);
                float height = MathFun.Lerp(x1, x2, cY);

                falloffMap[x, y] = new Vector2(height, mainInfluence);
            }
        }

        return falloffMap;
    }

    public static Vector2[,] BiomeMap(Vector2Int startPos, int size, Biome[] biomes, float scale, int seed)
    {
        Vector2[,] biomeMap = new Vector2[size, size];
        float bWeight = 1f / biomes.Length;

        for(int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float xSample = (startPos.x + x) / scale;
                float ySample = (startPos.y + z) / scale;
                float gNoise = (Noise.Noise2D(xSample, ySample, 216, seed) + 1f) * 0.5f;

                for(int b = 0; b < biomes.Length; b++)
                {
                    if (gNoise <= bWeight * (b + 1))
                    {
                        float val = Mathf.InverseLerp(bWeight * b, bWeight * (b + 1), gNoise);
                        int bIndex = b;
                        if (val < 0.25f)
                        {
                            val = biomes[b].biomeHeight;
                        }
                        else if (b < biomes.Length - 1)
                        {
                            val = Mathf.InverseLerp(0.25f, 1f, val);
                            if (val >= 0.15f) bIndex++;
                            val = MathFun.Lerp(biomes[b].biomeHeight, biomes[b + 1].biomeHeight, val);
                        }
                        else
                            val = biomes[b].biomeHeight;
                        biomeMap[x, z] = new Vector2(val, bIndex);
                        break;
                    }
                }
            }
        }

        return biomeMap;
    }

    public static float[,] HeightMap(Vector2Int startPos, int size, Biome[] biomes, GenRules noiseGen, int seed)
    {
        float[,] heightMap = new float[size, size];
        Vector2[,] biomeMap = BiomeMap(startPos, size, biomes, noiseGen.vScale, seed);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float pNoise = 0f;
                float freq = 1f;
                float amp = 1f;

                for (int o = 0; o < noiseGen.octaves; o++)
                {
                    float xSample = (startPos.x + x) / noiseGen.gScale * freq;
                    float ySample = (startPos.y + z) / noiseGen.gScale * freq;

                    float gNoise = (Noise.Noise2D(xSample, ySample, MathFun.Floor(MathFun.Power(3, o)) - 1, seed) + 1) * 0.5f;
                    pNoise += gNoise * amp;
                    amp *= noiseGen.amplify;
                    freq *= noiseGen.frequency;
                }

                heightMap[x, z] = pNoise * biomeMap[x, z].x;
            }
        }

        return heightMap;
    }

    public static byte[,,] ChunkMap(Vector2Int chunkPos, Biome[] biomes, GenRules noiseGen, int seed)
    {
        byte[,,] chunkData = new byte[Chunk.ChunkSize, Chunk.ChunkHeight, Chunk.ChunkSize];
        float[,] heightMap = HeightMap(chunkPos, Chunk.ChunkSize, biomes, noiseGen, seed);
        Vector2[,] biomeMap = BiomeMap(chunkPos, Chunk.ChunkSize, biomes, noiseGen.vScale, seed);

        for(int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int z = 0; z < Chunk.ChunkSize; z++)
            {
                int tHeight = MathFun.Round(heightMap[x, z] * noiseGen.growth) + noiseGen.minHeight;
                int biomeIndex = MathFun.Floor(biomeMap[x, z].y);

                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    byte useTile = 0;

                    if(y == 0)
                    {
                        useTile = 1;
                    }
                    else if(y < noiseGen.seaLevel && y > tHeight)
                    {
                        // Water
                        useTile = 4;
                    }
                    else if(y < tHeight - biomes[biomeIndex].secondaryDepth)
                    {
                        useTile = 1;
                    }
                    else if(y < tHeight)
                    {
                        useTile = biomes[biomeIndex].secondaryTile;
                    }
                    else if(y == tHeight)
                    {
                        useTile = biomes[biomeIndex].primaryTile;
                    }
                    chunkData[x, y, z] = useTile;
                }
            }
        }

        return chunkData;
    }
}
