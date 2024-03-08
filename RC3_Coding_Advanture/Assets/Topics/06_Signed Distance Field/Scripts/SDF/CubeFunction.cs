using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Vector3Extensions;

public class CubeFunction : ShapeFunction
{
    public Vector3 size = new Vector3(1, 1, 1);

    public override float Evaluate(Vector3 point)
    {
        var inPoint = transform.InverseTransformPoint(point);
        Vector3 o = Abs(inPoint - Vector3.zero) - size;
        float ud = Max(o, 0).magnitude;
        float n = Mathf.Max(Mathf.Max(Mathf.Min(o.x, 0), Mathf.Min(o.y, 0)), Mathf.Min(o.z, 0));
        return ud + n;
    }
}
