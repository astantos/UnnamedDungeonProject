using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralMapGenerator))]
public class ProceduralMapGeneratorEditor : Editor
{
    protected ProceduralMapGenerator generator;

    public override void OnInspectorGUI()
    {
        generator = (ProceduralMapGenerator)target;

        DrawDefaultInspector();
        
        if (GUILayout.Button("BEGIN GENERATION"))
        {
            if (Application.isPlaying)
            {
                generator.Generate();
            }
            else
            {
                Debug.LogWarning("Can only generate in PLAY mode");
            }
        }
    }
}
