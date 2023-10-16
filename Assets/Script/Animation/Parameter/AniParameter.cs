using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AniParameter : PropertyAttribute
{
    [SerializeField]
    public AniParameterType parameterType;
    [SerializeField]
    public string parameterName;

    [SerializeField]
    public bool parameterBool;
    [SerializeField]
    public float parameterFloat;
}

// Stage를 타게팅
[CustomPropertyDrawer(typeof(AniParameter))]
public class AniParameterEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, property.FindPropertyRelative("parameterName"));

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("parameterType"));

        if (property.FindPropertyRelative("parameterType").enumValueIndex == (int)AniParameterType.Bool)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("parameterBool"));
            EditorGUILayout.Space(10);
        }
        else if (property.FindPropertyRelative("parameterType").enumValueIndex == (int)AniParameterType.Float)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("parameterFloat"));
            EditorGUILayout.Space(10);
        }
        else if (property.FindPropertyRelative("parameterType").enumValueIndex == (int)AniParameterType.Trigger)
        {
            EditorGUILayout.Space(10);
        }
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
public enum AniParameterType
{
    Bool,
    Float,
    Trigger
}