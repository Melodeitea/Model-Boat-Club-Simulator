using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoatAutoPilot))]
public class BoatAutoPilotEditor : Editor
{
    private SerializedProperty behaviorProperty;
    private Editor behaviorEditor;

    private void OnEnable()
    {
        // cache the reference to the behavior ScriptableObject field
        behaviorProperty = serializedObject.FindProperty("behavior");
    }

    public override void OnInspectorGUI()
    {
        // always update the serialized object before drawing anything
        serializedObject.Update();

        // draw the default field for the behavior reference
        EditorGUILayout.PropertyField(behaviorProperty);

        // If a behavior asset is assigned, I draw its inspector inline
        if (behaviorProperty.objectReferenceValue != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Behavior Settings", EditorStyles.boldLabel);

            // create (or reuse) an editor for the ScriptableObject
            CreateCachedEditor(
                behaviorProperty.objectReferenceValue,
                null,
                ref behaviorEditor);

            // draw the ScriptableObject inspector directly inside this one
            EditorGUILayout.HelpBox(
            "These values come from a shared Behavior asset.\n" +
            "Modifying them affects all boats using it.",
            MessageType.Info);

            behaviorEditor.OnInspectorGUI();
        }
        // apply changes made through the inspector
        serializedObject.ApplyModifiedProperties();
    }
    
    
}
