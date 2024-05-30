using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFunction))]
public class WaveFunctionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var waveFunction = (WaveFunction)target;

        GUILayout.Space(10);

        GUILayout.Label("Algorithm Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Regenerate Wave Function")) waveFunction.RegenerateWaveFunction();

        if (GUILayout.Button("Run flood fill algorithm")) waveFunction.FloodFillWrapper();
    }
}