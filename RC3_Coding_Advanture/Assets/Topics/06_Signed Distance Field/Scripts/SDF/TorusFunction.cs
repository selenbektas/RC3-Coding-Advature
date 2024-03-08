using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusFunction : ShapeFunction
{

    public float radius1 = 1;
    public float radius2 = 3;

    public override float Evaluate(Vector3 point)
    {
        Vector3 inPoint = transform.InverseTransformPoint(point);

        var dir = inPoint;
        dir.z = 0;

        Vector3 q = new Vector3(dir.magnitude - radius1, inPoint.z , 0);

        return q.magnitude - radius2;
    }


}
