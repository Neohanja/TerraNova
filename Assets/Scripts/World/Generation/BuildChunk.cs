using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildChunk
{
    public static float[,] VornoiMap(Vector2Int startPos, int size, Biome[] biomes, NoiseLayer noiseGen, int seed)
    {
        float[,] noiseMap = new float[size, size];

        // (0, 0); (0, 1); (1, 0); (1, 1)
        float[] corners = new float[4];

        for (int cX = 0; cX < 2; cX++)
        {
            for (int cY = 0; cY < 2; cY++)
            {
                float sampleX = (startPos.x + cX * size) / noiseGen.scale;
                float sampleY = (startPos.y + cY * size) / noiseGen.scale;
                int bIndex = MathFun.Floor(Noise.VoronoiNoise(new Vector2(sampleX, sampleY), biomes.Length, seed).y);

                corners[cX + cY * 2] = biomes[bIndex].biomeHeight;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {

                float cX = MathFun.Curve(x / (float)size);
                float cY = MathFun.Curve(y / (float)size);
                float x1 = MathFun.Lerp(corners[0], corners[1], cX);
                float x2 = MathFun.Lerp(corners[2], corners[3], cX);
                float height = MathFun.Lerp(x1, x2, cY);

                noiseMap[x, y] = height * noiseGen.strength;
            }
        }

        return noiseMap;
    }

    
    public static float[,] GradMap(Vector2Int startPos, int size, NoiseLayer noiseGen, int seed)
    {
        float[,] biomeMap = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float xSample = (startPos.x + x) / noiseGen.scale;
                float ySample = (startPos.y + z) / noiseGen.scale;
                float gNoise = Noise.Noise2D(xSample, ySample, noiseGen.noiseOffset, seed);

                if(noiseGen.normalize)
                {
                    gNoise = (gNoise + 1) * 0.5f;
                }

                biomeMap[x, z] = gNoise * noiseGen.strength;
            }
        }

        return biomeMap;
    }

    public static float[,] HeightMap(Vector2Int startPos, int size, Biome[] biomes, GenRules noiseGen, int seed)
    {
        float[,] heightMap = new float[size, size];

        Vector2[,] biomeMap = BiomeMap(startPos, size, biomes, noiseGen.vScale, seed);

        List<float[,]> layerMap = new List<float[,]>();

        for(int l = 0; l < noiseGen.layer.Length; ++l)
        {
            switch(noiseGen.layer[l].noiseType)
            {
                case NoiseLayer.LayerType.Voronoi:
                    layerMap.Add(VornoiMap(startPos, size, biomes, noiseGen.layer[l], seed));
                    break;
                case NoiseLayer.LayerType.Grad:
                    layerMap.Add(GradMap(startPos, size, noiseGen.layer[l], seed));
                    break;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float pNoise = 0f;
                for (int l = 0; l < noiseGen.layer.Length; l++)
                {
                    switch(noiseGen.layer[l].ampType)
                    {
                        case NoiseLayer.LayerAmp.Add:
                            pNoise += layerMap[l][x, z];
                            break;
                        case NoiseLayer.LayerAmp.Multiply:
                            pNoise *= layerMap[l][x, z];
                            break;
                        case NoiseLayer.LayerAmp.PushFromZero:
                            float move = layerMap[l][x, z];
                            if (pNoise < 0)
                            {
                                if (move > 0)
                                    move *= -1;
                            }
                            else
                            {
                                if (move < 0)
                                    move *= -1;
                            }
                            pNoise += move;
                            break;
                    }                    
                }
                heightMap[x, z] = pNoise * biomeMap[x, z].x;
            }
        }

        return heightMap;
    }

    public static Vector2[,] BiomeMap(Vector2Int startPos, int size, Biome[] biomes, float scale, int seed)
    {
        Vector2[,] biomeMap = new Vector2[size, size];
        float bWeight = 1f / biomes.Length;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float xSample = (startPos.x + x) / scale;
                float ySample = (startPos.y + z) / scale;
                float gNoise = (Noise.Noise2D(xSample, ySample, 216, seed) + 1f) * 0.5f;

                for (int b = 0; b < biomes.Length; b++)
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
}
