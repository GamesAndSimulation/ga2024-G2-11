#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        GameManager gameManager = (GameManager)target;
        if(GUILayout.Button("Clear Player Prefs"))
        {
            gameManager.BeforeBuild();
        }
    }
}
#endif