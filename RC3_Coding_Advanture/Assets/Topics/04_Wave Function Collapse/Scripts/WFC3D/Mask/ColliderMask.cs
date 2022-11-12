using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMask : VolumetricMask
{
    [SerializeField] Collider[] colliders;

    public bool invert;


    public override bool IsMaskedTrue(Vector3 point)
    {
        if (invert)
        {
            return !IsPointInside(point);
        }
        else
        {
            return IsPointInside(point);
        }
    }


    bool IsPointInside(Vector3 point)
    {
        foreach(var collider in colliders)
        {
            if (collider.IsPointInside(point))
            {
                return true;
            }
        }

        return false;
    }

    

   
}
