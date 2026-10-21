using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Dialouge", menuName = "ScriptableObjects/dialouge")]
public class DialougeObject : ScriptableObject
{
    [System.Serializable]
    public struct Speech
    {
        public Sprite expression;
        public VideoClip videoExpression;

        [TextArea(5, 20)]
        public string text;

        public string[] answers;

        public string wrongText;

        public bool noWrongAnswers;

        public DialougeObject[] answerPaths;
    }

    public Speech[] Dialouge;
}


/*#if UNITY_EDITOR
[CustomEditor(typeof(DialougeObject))]
public class DialougeObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Dialouge"), true);
        serializedObject.ApplyModifiedProperties();
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ShowIfAttribute : PropertyAttribute
{
    public string[] conditions;
    public ShowIfAttribute(params string[] conditions) => this.conditions = conditions;
}


[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    private bool ShouldShow(SerializedProperty property)
    {
        var attr = (ShowIfAttribute)attribute;
        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');
        string containerPath = lastDot > 0 ? path.Substring(0, lastDot + 1) : "";

        foreach (string field in attr.conditions)
        {
            var cond = property.serializedObject.FindProperty(containerPath + field);
            if (cond == null || !cond.boolValue) return false;
        }
        return true;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return;

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }


        if (property.isArray)
        {
            // Header line
            float h = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                // Size field
                h += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Each element
                for (int i = 0; i < property.arraySize; i++)
                {
                    h += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i), true);
                    h += EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return h;
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif*/