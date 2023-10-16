using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;


public enum AniDirection : byte
{
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}
public struct AniInfo : IComponentData
{
    public AniStateType _aniState;
    public AniDirection aniDir;
    public int index;
    public float time;
    public float speed;
    public AniStateType aniState
    {
        get { return _aniState; }
        set 
        {
            if (_aniState != value)
                index = 0;

            _aniState = value; 
        }
    }
}

[MaterialProperty("_SpecColor")]
public struct AniEffectColor : IComponentData
{
    public float4 color;
}
[MaterialProperty("_BaseColorMap_TO")]
public struct AniTileOffset : IComponentData
{
    public float4 tileOffset;
}
public struct AniEffect : IComponentData
{
    public float time; // 0.3f ±îÁö.
    public int effectType; // 0 normal , 1 fire
    public float4 targetColor
    {
        get
        {
            if (time >= 0.17f)
                return new float4(0, 0, 0, 1);
            else if (effectType == 0)
                return new float4(1, 1, 1, 1);
            else if (effectType == 1)
                return new float4(1, 0, 0, 0.75f);
            else
                return new float4(1, 1, 1, 1);
        }
    }
}