using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildChunk
{
    public static float[,] VornoiMap(Vector2 startPos, int size, Biome[] biomes, NoiseLayer noiseGen, int seed)
    {
        float[,] noiseMap = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(startPos.x + x, startPos.y + y) / noiseGen.scale;
                Vector3[] gridLoc = Noise.VoronoiNoise(point, noiseGen.noiseOffset, biomes.Length, seed);

                float[] dist = new float[] { 2f, 2f, 2f };
                float[] bHeight = new float[] {1f, 1f, 1f };

                for(int v = 0; v < 9; v++)
                {
                    float newDist = Vector2.Distance(point, new Vector2(gridLoc[v].x, gridLoc[v].y));
                    float newHeight = gridLoc[v].z;

                    for (int i = 0; i < 3; i++)
                    {
                        if (dist[i] > newDist)
                        {
                            for (int r = 2; r > i; r--)
                            {
                                dist[r] = dist[r - 1];
                                bHeight[r] = bHeight[r - 1];
                            }
                            dist[i] = newDist;
                            bHeight[i] = newHeight;
                        }
                    }
                }

                float height = 0f;
                float tot = 0f;

                for(int a = 0; a < 3; a++)
                {
                    tot += dist[a];
                    height += MathFun.Lerp(0f, bHeight[a], dist[a]);
                }
                height /= 3;

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
                    float influence = 1f;

                    if(MathFun.Between(0, layerMap.Count - 1, noiseGen.layer[l].layerInfluence))
                    {
                        influence = layerMap[noiseGen.layer[l].layerInfluence][x, z];
                    }

                    switch(noiseGen.layer[l].ampType)
                    {
                        case NoiseLayer.LayerAmp.Add:
                            pNoise += layerMap[l][x, z] * influence;
                            break;
                        case NoiseLayer.LayerAmp.Multiply:
                            pNoise *= layerMap[l][x, z] * influence;
                            break;
                        case NoiseLayer.LayerAmp.PushFromZero:
                            float movePFZ = layerMap[l][x, z];
                            if (pNoise < 0)
                            {
                                if (movePFZ > 0)
                                    movePFZ *= -1;
                            }
                            else
                            {
                                if (movePFZ < 0)
                                    movePFZ *= -1;
                            }
                            pNoise += movePFZ * influence;
                            break;
                        case NoiseLayer.LayerAmp.Blend:
                            pNoise = MathFun.Lerp(pNoise, layerMap[l][x, z], noiseGen.layer[l].threshold);
                            break;
                        case NoiseLayer.LayerAmp.CracksAdd:
                            float moveCA = 0f;
                            if (noiseGen.layer[l].threshold >= MathFun.Abs(layerMap[l][x, z]))
                            {
                                moveCA = layerMap[l][x, z] * influence;
                            }
                            pNoise += moveCA;
                            break;
                        case NoiseLayer.LayerAmp.CracksMult:
                            float moveCM = 1f;
                            if (noiseGen.layer[l].threshold >= MathFun.Abs(layerMap[l][x, z]))
                            {
                                moveCM = layerMap[l][x, z] * influence;
                            }
                            pNoise *= moveCM;
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
