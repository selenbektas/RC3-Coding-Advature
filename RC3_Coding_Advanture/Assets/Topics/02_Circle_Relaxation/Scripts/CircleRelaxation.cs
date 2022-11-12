using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CircleRelaxation : MonoBehaviour
{
    public Circle circlePrefab;
    // number of circles
   public int NumCircles = 100;

    public int iterations = 10000;

    // min radius of circles
    public float min_radius = 1.0f;
    // max radius of circles
    public float max_radius = 3.0f;
    // range of center points be populated at the beginning
    public float range = 2.0f;

    public float boundRadius = 15f;

    public float momentum = 1.0f;

    public float threshold = 0.01f;


    List<Circle> circles;

    int currentIter = 0;

    // Start is called before the first frame update
    void Start()
    {
        PopulateCircles(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIter < iterations)
        {
            CheckCollisions();
            MoveCircles();
            currentIter++;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCircles();
        }
  
    }

    private void OnDrawGizmos()
    {
        
    }

    private void ResetCircles()
    {
        PopulateCircles();
        currentIter = 0;
    }

    void CheckCollisions()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            for (int j = i + 1; j < circles.Count; j++)
            {
                circles[i].CheckCollision(circles[j], momentum, threshold);
            }
            circles[i].CheckBound(boundRadius);
        }
          
    }

    void MoveCircles()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            circles[i].Move();
        }
    }


    void PopulateCircles(bool createNew=false)
    {
        //initialize the array of points and velocities

        if (createNew)
        {
            if (circles != null && circles.Count > 0)
            {
                while (circles.Count > 0)
                {
                    Destroy(circles[0]);
                    circles.RemoveAt(0);
                }
            }

            circles = new List<Circle>();
        }

        for (int i = 0; i < NumCircles; i++)
        {
            float x = Random.Range(-range, range);
         
            float z = Random.Range(-range, range);

            float r = Random.Range(min_radius, max_radius);

           var p = new Vector3(x, 0, z);

            Circle circle = null;

            if (i < circles.Count)
            {
                 circle = circles[i];
            }
            else
            {
                 circle = Instantiate(circlePrefab, transform);
                circles.Add(circle);
            }
            circle.Init(p, r);
        }
    }

}

public class CollisionCircle
{
    public Vector3 center;
    public float radius;
    public Vector3 totalMove = new Vector3(0, 0, 0);
    public Vector3 direction;
    public int collisionCounts = 0;
 
    public CollisionCircle(Vector3 _center, float _radius,int resolution=20)
    {
        center = _center;
        radius = _radius;
     

    }

   

    static IEnumerable<Vector3> GetCirclePoints(int count, float radius)
    {
        float degree = 2 * Mathf.PI / count;

        for (int i = 0; i < count; i++)
        {
            float angle = degree * i;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            yield return new Vector3(x, 0, z);
        }
    }
    public void Move()
    {
        if (collisionCounts != 0)
        {
            var m = totalMove / collisionCounts;
            center += m;
            direction = m.normalized;
        }

        totalMove = new Vector3(0, 0, 0);
        collisionCounts = 0;

    }

    public void DrawGizmosCircle()
    {
        Gizmos.DrawWireSphere(center, radius);
    }

    public void CheckCollision(CollisionCircle other, float momentum, float threshold)
    {
        var collisionDistance = radius + other.radius;
        var distance = Vector3.Distance(center, other.center);

        float moveValue = collisionDistance - distance;

        if (moveValue < threshold)
        {
            return;
        }

        var move = (center - other.center);

        move = move.normalized*momentum * moveValue;

        totalMove += move;
        other.totalMove -= move;
        collisionCounts++;
        other.collisionCounts++;
    }
}
