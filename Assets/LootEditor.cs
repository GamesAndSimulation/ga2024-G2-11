#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Loot))]
public class LootEditor : Editor
{
    SerializedProperty lootType;
    SerializedProperty essenceThing;
    SerializedProperty flyWoosh;
    SerializedProperty quantity;
    SerializedProperty ammoSound;
    SerializedProperty coinsSound;
    SerializedProperty essenceBloodSound;

    private void OnEnable()
    {
        lootType = serializedObject.FindProperty("lootType");
        essenceThing = serializedObject.FindProperty("EssenceThing");
        flyWoosh = serializedObject.FindProperty("flyWoosh");
        quantity = serializedObject.FindProperty("quantity");
        ammoSound = serializedObject.FindProperty("ammoSound");
        coinsSound = serializedObject.FindProperty("coinsSound");
        essenceBloodSound = serializedObject.FindProperty("essenceBloodSound");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(lootType);
        EditorGUILayout.PropertyField(quantity);
        EditorGUILayout.PropertyField(ammoSound);
        EditorGUILayout.PropertyField(coinsSound);
        EditorGUILayout.PropertyField(essenceBloodSound);

        if (lootType.enumValueIndex == (int)Loot.LootType.EssenceBlood)
        {
            EditorGUILayout.PropertyField(essenceThing);
            EditorGUILayout.PropertyField(flyWoosh);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif