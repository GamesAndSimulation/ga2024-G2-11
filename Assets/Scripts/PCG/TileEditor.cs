#if UNITY_EDITOR 

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileData tileData = (TileData)target;

        GUILayout.Space(10);
        GUILayout.Label("Setup Tools", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. Initialize Protoypes"))
        {
            tileData.ComputePrototypes();
            EditorUtility.SetDirty(tileData); // Mark the object as dirty to ensure changes are saved
        }
        
        if (GUILayout.Button("2. Compute Prototypes Neighbors"))
        {
            tileData.ComputePrototypesNeighbors();
            EditorUtility.SetDirty(tileData); // Mark the object as dirty to ensure changes are saved
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Debugging Tools", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Print Prototypes"))
        {
            tileData.PrintPrototypes();
        }
        
        if (GUILayout.Button("Print Prototypes Neighbors"))
        {
            tileData.PrintPrototypesNeighbors();
        }

    }

    //private TileData[] FindAllTileDataAssets()
    //{
    //    string[] guids = AssetDatabase.FindAssets("t:TileData");
    //    TileData[] tiles = new TileData[guids.Length];
    //    for (int i = 0; i < guids.Length; i++)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
    //        tiles[i] = AssetDatabase.LoadAssetAtPath<TileData>(path);
    //    }
    //    return tiles;
    //}
}
#endif
