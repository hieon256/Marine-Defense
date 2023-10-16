using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Rendering.FilterWindow;

public class AniStateMono : MonoBehaviour
{
    public AniTypeMono aniTypeData;
    public AniStateType aniStateType;
    //public List<AniCondition> aniConditions = new List<AniCondition>();
    public int row;
    public int column;

    public int frameCount;
    public float frameTime;

    public ObjectType objectType
    {
        get { return aniTypeData.objectType; }
    }
    public int characterIndex
    {
        get { return aniTypeData.characterIndex; }
    }
}
public class AniStateBaker : Baker<AniStateMono>
{
    public override void Bake(AniStateMono authoring)
    {
        var e = GetEntity(TransformUsageFlags.None);

        if (authoring.row == 0 || authoring.column == 0)
            Debug.LogError(authoring.name + "'s row or col is zero");

        AddComponent(e, new AniStateData
        {
            objectType = authoring.objectType,
            characterIndex = authoring.characterIndex,
            stateType = authoring.aniStateType,
            row = authoring.row, 
            column = authoring.column,
            frameCount = authoring.frameCount,
            frameTime = authoring.frameTime
        });
    }
}
public enum AniStateType : byte
{
    Idle,
    Move,
    Dead,
    Action1,
    Action2,
    Action3
}
public struct AniStateData: IComponentData
{
    public ObjectType objectType;
    public int characterIndex;
    public AniStateType stateType;

    public int row;
    public int column;

    public int frameCount;
    public float frameTime;
}
/*
[CustomEditor(typeof(AniStateMono))]
public class AniStateEditor : Editor
{
    private AniStateMono _aniStateData;

    private SerializedProperty conditions;

    private ReorderableList _conditions;

    private GUIContent[] availableOptions;
    private void OnEnable()
    {
        _aniStateData = (AniStateMono)target;

        conditions = serializedObject.FindProperty(nameof(AniStateMono.aniConditions));

        _conditions = new ReorderableList(serializedObject, conditions)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true, // for the dialogue items we can allow re-ordering

            // As the header we simply want to see the usual display name of the DialogueItems
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, conditions.displayName),

            // How shall elements be displayed
            drawElementCallback = (rect, index, focused, active) =>
            {
                // get the current element's SerializedProperty
                var element = conditions.GetArrayElementAtIndex(index);

                // Get the nested property fields of the DialogueElement class
                var parameter = element.FindPropertyRelative(nameof(AniCondition.parameterId));

                var popUpHeight = EditorGUI.GetPropertyHeight(parameter);

                // store the original GUI.color
                var color = GUI.color;

                // if the value is invalid tint the next field red
                if (parameter.intValue < 0) GUI.color = Color.red;

                // Draw the Popup so you can select from the existing character names
                parameter.intValue = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, popUpHeight), new GUIContent(parameter.displayName), parameter.intValue, availableOptions);

                // reset the GUI.color
                GUI.color = color;
                rect.y += popUpHeight;

                if (parameter.intValue == (int)AniParameterType.Bool)
                {
                    var parameterValue = element.FindPropertyRelative(nameof(AniCondition.boolConditionValue));

                    var valueHeight = EditorGUI.GetPropertyHeight(parameterValue);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, valueHeight), parameterValue);
                }
                else if (parameter.intValue == (int)AniParameterType.Float)
                {
                    var parameterCondition = element.FindPropertyRelative(nameof(AniCondition.floatConditionType));

                    var conditionHeight = EditorGUI.GetPropertyHeight(parameterCondition);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, conditionHeight), parameterCondition);

                    var parameterValue = element.FindPropertyRelative(nameof(AniCondition.floatConditionValue));

                    var valueHeight = EditorGUI.GetPropertyHeight(parameterValue);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, valueHeight), parameterValue);
                }
                else if (parameter.intValue == (int)AniParameterType.Trigger)
                {
                }
            },

            // Get the correct display height of elements in the list
            // according to their values
            // in this case e.g. we add an additional line as a little spacing between elements
            elementHeightCallback = index =>
            {
                var element = conditions.GetArrayElementAtIndex(index);

                var parameter = element.FindPropertyRelative(nameof(AniCondition.parameterId));

                if (parameter.intValue == (int)AniParameterType.Bool)
                {
                    var parameterValue = element.FindPropertyRelative(nameof(AniCondition.boolConditionValue));

                    return EditorGUI.GetPropertyHeight(parameter) + EditorGUI.GetPropertyHeight(parameterValue) + EditorGUIUtility.singleLineHeight;
                }
                else if (parameter.intValue == (int)AniParameterType.Float)
                {
                    var parameterCondition = element.FindPropertyRelative(nameof(AniCondition.floatConditionType));
                    var parameterValue = element.FindPropertyRelative(nameof(AniCondition.floatConditionValue));

                    return EditorGUI.GetPropertyHeight(parameter) + EditorGUI.GetPropertyHeight(parameterCondition) + EditorGUI.GetPropertyHeight(parameterValue) + EditorGUIUtility.singleLineHeight;
                }
                else if (parameter.intValue == (int)AniParameterType.Trigger)
                {
                }

                return EditorGUI.GetPropertyHeight(parameter) + EditorGUIUtility.singleLineHeight;
            },

            // Overwrite what shall be done when an element is added via the +
            // Reset all values to the defaults for new added elements
            // By default Unity would clone the values from the last or selected element otherwise
            onAddCallback = list =>
            {
                // This adds the new element but copies all values of the select or last element in the list
                list.serializedProperty.arraySize++;

                var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                var parameter = newElement.FindPropertyRelative(nameof(AniCondition.parameterId));

                parameter.intValue = -1;

                if (parameter.intValue == (int)AniParameterType.Bool)
                {
                    var parameterValue = newElement.FindPropertyRelative(nameof(AniCondition.boolConditionValue));

                    parameterValue.boolValue = false;
                }
                else if (parameter.intValue == (int)AniParameterType.Float)
                {
                    var parameterCondition = newElement.FindPropertyRelative(nameof(AniCondition.floatConditionType));
                    var parameterValue = newElement.FindPropertyRelative(nameof(AniCondition.floatConditionValue));

                    parameterCondition.enumValueIndex = 0;
                    parameterValue.floatValue = 0f;
                }
                else if (parameter.intValue == (int)AniParameterType.Trigger)
                {
                }
            }
        };

        // Get the existing character names ONCE as GuiContent[]
        // Later only update this if the charcterList was changed
        availableOptions = _aniStateData.aniParameters.Select(item => new GUIContent(item.parameterName)).ToArray();
    }
    public override void OnInspectorGUI()
    {
        DrawScriptField();

        // load real target values into SerializedProperties
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (EditorGUI.EndChangeCheck())
        {
            // Write back changed values into the real target
            serializedObject.ApplyModifiedProperties();

            // Update the existing character names as GuiContent[]
            availableOptions = _aniStateData.aniParameters.Select(item => new GUIContent(item.parameterName)).ToArray();
        }

        _conditions.DoLayoutList();

        // Write back changed values into the real target
        serializedObject.ApplyModifiedProperties();
    }
    private void DrawScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((AniStateMono)target), typeof(AniStateMono), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
    }
}*/