#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Portal))]
public class PortalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Portal myComponent = (Portal)target;

        myComponent.isEntrance = EditorGUILayout.Toggle("Enable Field", myComponent.isEntrance);

        using (new EditorGUI.DisabledScope(!myComponent.isEntrance))
        {
            myComponent.linkedPortal = (Transform)EditorGUILayout.ObjectField("Linked Portal", myComponent.linkedPortal, typeof(Transform), true);
        }
    }
}
#endif