using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveFunction))]
public class WaveFunctionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaveFunction waveFunction = (WaveFunction)target;
        
        GUILayout.Space(10);
        
        if(GUILayout.Button("Regenerate Wave Function"))
        {
            waveFunction.RegenerateWaveFunction();
        }
    }
}