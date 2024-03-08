using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDFShape : MonoBehaviour
{

    public enum ShapeType {Sphere,Cube,Torus};
    public enum Operation {None, Blend, Cut,Mask}

    public ShapeType shapeType;
    public Operation operation;
    public Color colour = Color.white;
    [Range(0,1)]
    public float blendStrength;
    [HideInInspector]
    public int numChildren;

    public Vector3 Position {
        get {
            return transform.position;
        }
    }

    public Vector3 Scale {
        get {
            Vector3 parentScale = Vector3.one;
            if (transform.parent != null && transform.parent.GetComponent<SDFShape>() != null) {
                parentScale = transform.parent.GetComponent<SDFShape>().Scale;
            }
            return Vector3.Scale(transform.localScale, parentScale);
        }
    }
}
