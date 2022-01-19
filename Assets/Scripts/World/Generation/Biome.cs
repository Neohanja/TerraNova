using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Terra/Biome Data")]
public class Biome : ScriptableObject
{
    public string biomeName;
    public float biomeHeight;
    public byte primaryTile;
    public byte secondaryTile;
    public int secondaryDepth;

    public Flora[] biomeFlora;
}

[System.Serializable]
public class Flora
{
    public string floraName;
    public byte trunk;
    public byte foliage;
    public int placementOffset;
    [Range(0f, 1f)]
    public float placementChance;
    public int growthOffset;
    public int minHeight;
    public int growth;
}
