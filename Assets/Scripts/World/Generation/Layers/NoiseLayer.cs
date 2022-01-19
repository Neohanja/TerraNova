using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer
{
    public float scale;
    [Range(-2f, 2f)]
    public float strength;
    public int noiseOffset;
    public bool normalize;
    public int layerInfluence = -1;
    public LayerAmp ampType;
    public LayerType noiseType;

    public enum LayerAmp
    {
        Add,
        PushFromZero,
        Multiply
    }

    public enum LayerType
    {
        Voronoi,
        Grad
    }
}