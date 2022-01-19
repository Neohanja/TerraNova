using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer
{
    public float scale;
    [Range(-1f, 1f)]
    public float strength;
    public int noiseOffset;
    public bool normalize;
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