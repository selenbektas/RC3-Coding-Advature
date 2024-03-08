using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFunction : ShapeFunction
{
    public float radius = 1;
    public override float Evaluate(Vector3 point)
    {
        var inPoint = transform.InverseTransformPoint(point);
        return  inPoint.magnitude - radius;
       
    }
}
