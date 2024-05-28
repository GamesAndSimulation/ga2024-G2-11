using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Tile tile = (Tile)target;

        if (GUILayout.Button("Initialize Sides"))
        {
            tile.InitializeSides();
            EditorUtility.SetDirty(tile); // Mark the object as dirty to ensure changes are saved
        }

        if (GUILayout.Button("Compute Neighbors"))
        {
            tile.ComputeAllNeighbors();
            EditorUtility.SetDirty(tile); // Mark the object as dirty to ensure changes are saved
        }
    }
}