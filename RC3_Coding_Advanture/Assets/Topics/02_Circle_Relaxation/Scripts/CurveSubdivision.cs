using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveSubdivision : MonoBehaviour
{

    public LineRenderer line;
    public int initialPointCount = 30;
    public float initialPointsRadius = 5f;
    public int maxPointCount = 300;
    public int maxIteration = 10000;

    public float radius = 2.0f;

    Vector3[] initialPositions;

    List<Vector3> centers = new List<Vector3>();

    int currentIter = 0;

    // Start is called before the first frame update
    void Start()
    {
        initialPositions = new Vector3[line.positionCount];
        line.GetPositions(initialPositions);
        Init();
    }

    // Update is called once per frame
    void Update()
    {

        if (currentIter < maxIteration)
        {
            Subdivide();
            UpdateLinePoints();
        }
 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Init();
        }
    }

    void Init()
    {
        centers = new List<Vector3>(initialPositions);
        currentIter = 0;
    }

    

    void UpdateLinePoints()
    {
        line.positionCount = centers.Count;
        line.SetPositions(centers.ToArray());
    }

    IEnumerable<Vector3> GetCirclePoints(int count,float radius)
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

    void Subdivide()
    {
        List<Vector3> totalMoves = new List<Vector3>();
        List<float> collisionCounts = new List<float>();

        for (int i = 0; i < centers.Count; i++)
        {
            totalMoves.Add(new Vector3(0, 0, 0));
            collisionCounts.Add(0);
        }

        float collisionDistance = radius;

        for (int i = 0; i < centers.Count; i++)
        {
            for (int j = i + 1; j < centers.Count; j++)
            {
                Vector3 move = centers[i] - centers[j];
                float d = move.magnitude;

                if (d > collisionDistance||d<0.001f)
                {
                    continue;
                }

                move = 0.5f * (collisionDistance - d) * move.normalized;
                totalMoves[i] += move;
                totalMoves[j] -= move;
                collisionCounts[i] += 1.0f;
                collisionCounts[j] += 1.0f;
            }
        }
          

        for (int i = 0; i < centers.Count; i++)
        {
            if (collisionCounts[i] > 0.0f)
                centers[i] += totalMoves[i] / collisionCounts[i];
        }
           

        //insert subdivision points if distance > radius
        if (centers.Count < maxPointCount)
        {
            List<int> splitIndices = new List<int>();

            for (int i = 0; i < centers.Count - 1; i++)
            {
                if ( Vector3.Distance( centers[i],centers[i + 1]) > radius)
                {
                    splitIndices.Add(i + 1 + splitIndices.Count);
                }
                 
            }
            
            foreach (int splitIndex in splitIndices)
            {
                Vector3 newCenter = 0.5f * (centers[splitIndex - 1] + centers[splitIndex]);
                centers.Insert(splitIndex, newCenter);
            }
        }
    }
}


