using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridBasedMapGenerator))]
public class GridBasedMapGeneratorEditor : Editor
{
    protected GridBasedMapGenerator generator;

    public override void OnInspectorGUI()
    {
        generator = (GridBasedMapGenerator)target;

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
