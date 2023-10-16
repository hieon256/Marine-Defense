using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AniCondition
{
    [SerializeField]
    public int parameterId;

    //float type
    [SerializeField]
    public FloatConditionType floatConditionType;
    [SerializeField]
    public float floatConditionValue;

    //bool type
    [SerializeField]
    public bool boolConditionValue;
}

// property.FindPropertyRelative("parameter").objectReferenceValue is AniParameter
[Serializable]
public enum FloatConditionType
{
    greater,
    less
}