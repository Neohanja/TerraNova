using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Terra/Shape Data")]
public class Shapes : ScriptableObject
{
    public string shapeName;
    public Face topFace;
    public Face bottomFace;
    public Face northFace;
    public Face eastFace;
    public Face southFace;
    public Face westFace;

    public Face GetFace(int face)
    {
        switch(face)
        {
            case 0:
                return topFace;
            case 1:
                return bottomFace;
            case 2:
                return northFace;
            case 3:
                return eastFace;
            case 4:
                return southFace;
            case 5:
                return westFace;
        }

        return null;
    }
}
