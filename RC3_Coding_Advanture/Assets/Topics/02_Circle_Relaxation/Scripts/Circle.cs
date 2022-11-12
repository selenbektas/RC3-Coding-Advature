using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public LineRenderer  line;
    public Vector3 center=Vector3.zero;
    public float radius = 1.0f;
    public Vector3 totalMove = new Vector3(0, 0, 0);
    public Vector3 direction;
    public int collisionCounts = 0;

    public void Init(Vector3 _center, float _radius, int resolution = 20)
    {
        center = _center;
        radius = _radius;
        transform.position = _center;
        //transform.localScale = Vector3.one* _radius * 2;

        line.positionCount = 30;
        
        line.SetPositions(GetCirclePoints(30, radius));
       
    }



    static Vector3[] GetCirclePoints(int count, float radius)
    {
        Vector3[] points = new Vector3[count];
        float degree = 2 * Mathf.PI / count;

        for (int i = 0; i < count; i++)
        {
            float angle = degree * i;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

           points[i] =new Vector3(x, 0, z);
        }

        return points;
    }
    public void Move()
    {
        if (collisionCounts != 0)
        {
            var m = totalMove / collisionCounts;
            center += m;
            direction = m.normalized;
            transform.position = center;
           
        }

        totalMove = new Vector3(0, 0, 0);
        collisionCounts = 0;

    }

    public void CheckCollision(Circle other, float momentum, float threshold)
    {
        var collisionDistance = radius + other.radius;
        var distance = Vector3.Distance(center, other.center);

        float moveValue = collisionDistance - distance;

        if (moveValue < threshold)
        {
            return;
        }

        var move = (center - other.center);

        move = move.normalized * momentum * moveValue;

        totalMove += move;
        other.totalMove -= move;
        collisionCounts++;
        other.collisionCounts++;
    }

    public void CheckBound(float boundRadius)
    {
        if (center.magnitude + radius > boundRadius)
        {
            totalMove = -totalMove;
            center = center.normalized * (boundRadius - radius);
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    var col = Gizmos.color;
    //    Gizmos.color = Color.red;

    //    Gizmos.DrawWireSphere(center, radius);

    //    Gizmos.color = col;
    //}

}
