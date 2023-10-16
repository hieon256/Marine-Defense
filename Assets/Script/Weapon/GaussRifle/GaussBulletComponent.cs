using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GaussBulletTag : IComponentData
{

}
public struct GaussBulletParticle : IComponentData
{
    public Entity particleEntity;
}
public struct GaussBulletRange : IComponentData
{
    public float range;
}