using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Face
{
    public Vector3[] vertices;
    public int[] trianges;
    public Vector2[] uvMap;

    public bool skipDraw;
}
