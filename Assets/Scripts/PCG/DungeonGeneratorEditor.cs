using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGenerator dungeonGenerator = (DungeonGenerator)target;

        if (GUILayout.Button("Generate Dungeon"))
        {
            dungeonGenerator.GenerateDungeon();
        }
        if(GUILayout.Button("Clear Dungeon"))
        {
            dungeonGenerator.ClearDungeon();
        }
    }
}
