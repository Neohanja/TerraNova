using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Landmass))]
public class LandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Landmass landmass = (Landmass)target;

        if(DrawDefaultInspector())
        {
            if(landmass.autoUpdate)
            {
                landmass.GenerateMap();
            }
        }

        if(GUILayout.Button("Generate Map"))
        {
            landmass.GenerateMap();
        }

        if(GUILayout.Button("Get Gen Rules"))
        {
            landmass.worldGen = landmass.worldEditor.worldGen;
        }

        if(GUILayout.Button("Push Gen Rules"))
        {
            landmass.worldEditor.worldGen = landmass.worldGen;
        }
    }
}
