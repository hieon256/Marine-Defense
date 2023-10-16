using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public readonly partial struct EnemyEffectAspect : IAspect
{
    private readonly RefRW<AniEffectColor> _enemySpecColor;
    private readonly RefRW<AniEffect> _enemyEffect;
    public float4 enemySpecColor
    {
        get => _enemySpecColor.ValueRO.color;
        set => _enemySpecColor.ValueRW.color = value;
    }
    public void AnimateEffect(float deltaTime)
    {
        float time = _enemyEffect.ValueRO.time + deltaTime;

        _enemyEffect.ValueRW.time = time;

        if (time > 0.34f)
            return;

        if (time > 0.17f)
            time -= 0.17f;

        float4 diffColor = _enemyEffect.ValueRO.targetColor - enemySpecColor;

        enemySpecColor += diffColor * time / 0.17f;

    }
}
