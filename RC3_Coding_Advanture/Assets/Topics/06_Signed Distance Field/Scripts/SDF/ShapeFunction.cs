using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeFunction : MonoBehaviour
{
    public ShapeOperation operation;
    public Material renderMaterial;

    public virtual float Evaluate(Vector3 point)
    {
        return 0;
    }

    public void RenderShape(RenderTexture source,RenderTexture destination)
    {
        Graphics.Blit(source, destination, renderMaterial);
    }
   
}

public enum ShapeOperation
{
    None, Blend, Cut, Mask
}

public static class Vector3Extensions
{
    public static Vector3 Abs( Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static Vector3 Max(params Vector3[] points)
    {
        float xMax = float.MinValue;
        float yMax = float.MinValue;
        float zMax = float.MinValue;

        for (int i = 0; i < points.Length; i++)
        {
            xMax = Mathf.Max(xMax, points[i].x);
            yMax = Mathf.Max(yMax, points[i].y);
            zMax = Mathf.Max(zMax, points[i].z);
        }

        return new Vector3(xMax, yMax, zMax);
    }

    public static Vector3 Max(Vector3 point,float val)
    {
        return new Vector3(Mathf.Max(point.x, val), Mathf.Max(point.y, val), Mathf.Max(point.z, val));
    }

    public static Vector3 Min(params Vector3[] points)
    {
        float xMin = float.MaxValue;
        float yMin = float.MaxValue;
        float zMin = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            xMin = Mathf.Min(xMin, points[i].x);
            yMin = Mathf.Min(yMin, points[i].y);
            zMin = Mathf.Min(zMin, points[i].z);
        }

        return new Vector3(xMin, yMin, zMin);
    }

    public static Vector3 Min(Vector3 point, float val)
    {
        return new Vector3(Mathf.Min(point.x, val), Mathf.Min(point.y, val), Mathf.Min(point.z, val));
    }
}
