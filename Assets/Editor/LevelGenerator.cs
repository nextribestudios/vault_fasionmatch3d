using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGeneratorController))]
public class LevelGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGeneratorController generatorController = (LevelGeneratorController)target;
        if(GUILayout.Button("Save Level"))
        {
            generatorController.SerializeToJson();
        }
    }
}
