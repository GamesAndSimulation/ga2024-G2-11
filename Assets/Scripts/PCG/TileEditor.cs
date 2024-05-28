using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileData tileData = (TileData)target;

        if (GUILayout.Button("Initialize Sides"))
        {
            tileData.InitializeSides();
            EditorUtility.SetDirty(tileData); // Mark the object as dirty to ensure changes are saved
        }

        if (GUILayout.Button("Compute Neighbors"))
        {
            TileData[] availableTiles = FindAllTileDataAssets();
            tileData.ComputeAllNeighbors(availableTiles);
            EditorUtility.SetDirty(tileData); // Mark the object as dirty to ensure changes are saved
        }
    }

    private TileData[] FindAllTileDataAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:TileData");
        TileData[] tiles = new TileData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            tiles[i] = AssetDatabase.LoadAssetAtPath<TileData>(path);
        }
        return tiles;
    }
}