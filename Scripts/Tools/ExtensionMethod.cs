using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f; // 调整这个值以改变视野范围
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {

        var vectorToTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        return dot >= dotThreshold; 
    }
}
