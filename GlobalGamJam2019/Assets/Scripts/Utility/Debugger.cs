using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Debugger
{
    public static void DrawCustomDebugRay(Vector3 rayStartPosition, Vector3 rayDirection, Color rayColor)
    {
        Debug.DrawRay(rayStartPosition, rayDirection, rayColor);
    }

    public static void DrawMultipleCustomDebugRayMultipleDirections(Vector3 rayStartPosition, Vector3[] rayDirections, Color rayColor)
    {
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Debug.DrawRay(rayStartPosition, rayDirections[i], rayColor);
        }
    }

    public static void DrawMultipleCustomDebugRayMultipleStarts(Vector3[] rayStartPosition, Vector3 rayDirection, Color rayColor)
    {
        for (int i = 0; i < rayStartPosition.Length; i++)
        {
            Debug.DrawRay(rayStartPosition[i], rayDirection, rayColor);
        }
    }

    public static void DrawMultipleCustomDebugRayMultipleStartsWithOffset(Vector3[] rayStartPosition, Vector3 rayDirection, Vector3 rayRelative, float rayOffset, Color rayColor)
    {
        for (int i = 0; i < rayStartPosition.Length; i++)
        {
            Vector3 rayPosition = rayStartPosition[i];
            rayPosition += (rayRelative * rayOffset);

            Debug.DrawRay(rayPosition, rayDirection, rayColor);
        }
    }

    public static void DrawNavMeshPath(NavMeshPath navMeshPath, Color debugColor)
    {
        for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], debugColor);
        }
    }
}

[System.Serializable]
public struct DebugShape
{
    [Header("Debug Shape Attributes")]
    public DEBUG_SHAPE_TYPE debugShapeType;

    [Space(10)]
    public Vector3 debugShapeSize;

    [Space(10)]
    public float debugShapeRadius;

    [Space(10)]
    public Transform associatedTransform;

    public DebugShape(DEBUG_SHAPE_TYPE newDebugShapeType, Vector3 newDebugShapeSize, float newDebugShapeRadius, Transform newDebugShapeAssociatedTransform)
    {
        this.debugShapeType = newDebugShapeType;
        this.debugShapeSize = newDebugShapeSize;
        this.debugShapeRadius = newDebugShapeRadius;
        this.associatedTransform = newDebugShapeAssociatedTransform;
    }
}

public enum DEBUG_SHAPE_TYPE
{
    NONE,
    BOX,
    SPHERE
}
