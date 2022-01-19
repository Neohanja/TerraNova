using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenRules
{
    public float vScale;
    public int minHeight;
    public int growth;
    public int seaLevel;

    [Header("Layers")]
    public NoiseLayer[] layer;
}