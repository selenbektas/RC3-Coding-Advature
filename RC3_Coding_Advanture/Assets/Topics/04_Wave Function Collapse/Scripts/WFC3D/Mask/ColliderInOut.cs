using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderInOut 
{
    static bool IsPointInside(this MeshCollider collider,Vector3 point,float maxDistance=1000f)
    {

        if (collider.convex == true)
        {
            var closest = collider.ClosestPoint(point);

            float d = (closest - point).sqrMagnitude;

            if (d > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        Vector3 dir = Random.insideUnitSphere;
       
        bool _break = false;

        Vector3 from = point;

        RaycastHit hit;

        Ray ray;

        int hitcount = 0;

        Physics.queriesHitBackfaces = true;
        while (!_break)
        {
            _break = true;
            ray = new Ray(from, dir);
            bool hasHit = collider.Raycast(ray, out hit, maxDistance);

            if (hasHit)
            {
                from = hit.point + 0.001f * dir;
                hitcount++;
                _break = false;
            }
        }
        Physics.queriesHitBackfaces = false;

        if (hitcount % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    public static bool IsPointInside(this Collider collider, Vector3 point, float maxDistance = 1000f)
    {
        if (collider.GetType().IsAssignableFrom(typeof(MeshCollider)))
        {
            return IsPointInside(collider as MeshCollider, point, maxDistance);
        }
        else
        {
            var closest = collider.ClosestPoint(point);

            float d = (closest - point).sqrMagnitude;

            if (d > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
      

    }


}
