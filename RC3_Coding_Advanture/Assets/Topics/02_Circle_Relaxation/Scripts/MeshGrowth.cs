using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MeshGrowth : MonoBehaviour
{
    [Range(0, 3)]
    public float edgeLengthWeight;
    [Range(0, 3)]
    public float collisionWeight;
    [Range(0, 3)]
    public float collisionDistance;
    [Range(0, 3)]
    public float bendingResistance;
    [Range(0, 3)]
    public float boundWeight;

    [Range(0, 3)]
    public float smoothWeight = 0.1f;

    public bool useField = false;

    public float stiffness = 10;

    public float jitter = 1.0f;

    public Vector3 noiseScale = Vector3.one;
    public Vector3 noiseOffset = Vector3.zero;

    public int maxVertexCount = 2000;

    public int maxIteration = 10000;

    public MeshFilter m_filter;
    public Collider bound;

    Mesh sourceMesh;

    MeshGrowther growther;

    List<Vector3> vertices;
    List<int> triangles;

    Mesh displayMesh;

    int currentIter = 0;


    // Start is called before the first frame update
    void Start()
    {
        Init();
        StartCoroutine(Grow());
    }

    // Update is called once per frame
     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            Init();
            StartCoroutine(Grow());
        }
    }

    IEnumerator Grow()
    {
        while (currentIter < maxIteration)
        {
            yield return growther.StepAsync(true, useField, Time.deltaTime * stiffness);
            UpdateMesh();
            currentIter++;
            yield return null;
        }
    }

    void Init()
    {

        if (sourceMesh == null)
        {
            sourceMesh= WeldMesh(m_filter.sharedMesh, 0.01f);
        }
   
        var hm = CreateFromMesh(sourceMesh, jitter);

        growther = new MeshGrowther(hm, edgeLengthWeight, collisionWeight, collisionDistance, maxVertexCount, bendingResistance, boundWeight,smoothWeight);

        growther.SetupField(noiseScale, noiseOffset);
        growther.bound = bound;

        vertices = new List<Vector3>();
        triangles = new List<int>();

        displayMesh = new Mesh();

        m_filter.sharedMesh = displayMesh;
        currentIter = 0;
    }

    void UpdateMesh()
    {
        var hm = growther.hemesh;

        vertices.Clear();
        triangles.Clear();

        for (int vi = 0; vi < hm.Vertices.Count; vi++)
        {
            vertices.Add((Vector3)hm.Vertices[vi].Position);
        }

        for (int fi = 0; fi < hm.Faces.Count; fi++)
        {
            var face = hm.Faces[fi];
            foreach (var v in face.Vertices)
            {
                triangles.Add(v.Index);
            }
        }


        displayMesh.SetVertices(vertices);
        displayMesh.triangles = triangles.ToArray();
        displayMesh.RecalculateNormals();
        displayMesh.RecalculateTangents();
    }

    static Mesh WeldMesh(Mesh mesh,float threshold=0.001f)
    {
        var vertices =new List<Vector3>(mesh.vertices);
        var triangles = new List<int>(mesh.triangles);


        var indices = vertices.Select((v, i) => i).ToList();
        var triangles_new = new List<int>();

        Dictionary<int, int> replacement = new Dictionary<int, int>();

        for (int i = 0; i < vertices.Count; i++)
        {
            List<int> toReplace = new List<int>();
            for (int j = i+1; j < vertices.Count; j++)
            {
                var v0 = vertices[i];
                var v1 = vertices[j];

                var d = (v0 - v1).sqrMagnitude;

                if (d < threshold)
                {
                    toReplace.Add(j);
                    indices.Remove(j);
                }
            }

            foreach (int re in toReplace)
            {
                if (!replacement.ContainsKey(re))
                {
                    replacement.Add(re, i);
                }
            }
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            int vi = triangles[i];

            if (replacement.ContainsKey(vi))
            {
                int vi_new = replacement[vi];
                triangles[i] = vi_new;
            }
        }

        replacement.Clear();

        for (int i = 0; i < indices.Count; i++)
        {
            int oldID = indices[i];
            int newID = i;

            replacement.Add(oldID, newID);
        }

        for (int i = 0; i < triangles.Count; i++)
        {
            int vi = triangles[i];

            if (replacement.ContainsKey(vi))
            {
                int vi_new = replacement[vi];
                triangles[i] = vi_new;
            }
        }

        vertices.GetSelection(indices);

        var mesh_new = new Mesh();
        mesh_new.SetVertices(vertices.GetSelection(indices));
        mesh_new.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        return mesh_new;
    }

    static HeMesh3d CreateFromMesh(Mesh mesh, float jitter = 0)
    {
        HeMesh3d hm = new HeMesh3d();

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            var v = hm.AddVertex();

            var p = mesh.vertices[i];

            p.x += jitter * (Random.value  - 1)*2;
            p.y += jitter * (Random.value  - 1)*2;
            p.z += jitter * (Random.value  - 1)*2;

            v.Position = p;
        }

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            hm.AddFace(v0, v1, v2);
        }

        return hm;

    }
}

public class MeshGrowther
{
    public HeMesh3d hemesh;
    public Collider bound;
    public PerlinNoiseField field;


    public double edgeLengthWeight;
    public double collisionWeight;
    public double collisionDistanceBase;
    public double bendingResistance;
    public double boundWeight;
    public double smoothWeight;

    public int maxVertexCount = 2000;

    List<Vec3d> totalWeightedMoves;
    List<double> totalWeight;
    public List<double> collisionDistances;

    public MeshGrowther(HeMesh3d mesh, double edgeLengthWeight, double collisionWeight, double collisionDistance, int maxVertexCount, double bendingResistance, double boundWeight,double smoothWeight)
    {
        hemesh = mesh;

        this.edgeLengthWeight = edgeLengthWeight;
        this.collisionWeight = collisionWeight;
        this.collisionDistanceBase = collisionDistance;
        this.maxVertexCount = maxVertexCount;
        this.bendingResistance = bendingResistance;
        this.boundWeight = boundWeight;
        this.smoothWeight = smoothWeight;


        totalWeightedMoves = new List<Vec3d>();
        totalWeight = new List<double>();
        collisionDistances = new List<double>();
        collisionDistances.Add(collisionDistanceBase);
    }

    public void SetupField(Vec3d scale, Vec3d offset)
    {
        field = new PerlinNoiseField(scale, offset);
    }

    public IEnumerator  StepAsync(bool grow, bool useField, double deltaTime)
    {
        if (grow)
        {
            SplitLongEdges();
        }

        totalWeightedMoves = new List<Vec3d>();
        totalWeight = new List<double>();

 
        for (int i = 0; i < hemesh.Vertices.Count; i++)
        {
            totalWeightedMoves.Add(new Vector3(0, 0, 0));

            totalWeight.Add(0);
        }

     
        if (field != null && useField)
        {
            collisionDistances = new List<double>();
            GetDistanceFromField(field);
            yield return null;
        }

        ProcessLengthConstraint();
        yield return null;

        ProcessBoundCollision();
        yield return null;

        ProcessCollision();
        yield return null;
        ProcessBendingResistance();
        yield return null;

        UpdatePosition(deltaTime);

    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="grow"></param>
    /// <returns></returns>

    public void Step(bool grow, bool useField, double deltaTime)
    {
        if (grow)
        {
            SplitLongEdges();
        }

        totalWeightedMoves = new List<Vec3d>();
        totalWeight = new List<double>();


        for (int i = 0; i < hemesh.Vertices.Count; i++)
        {
            totalWeightedMoves.Add(new Vector3(0, 0, 0));
            totalWeight.Add(0);
            //collisionDistances.Add(collisionDistanceBase);
        }
        if (field != null && useField)
        {
            collisionDistances = new List<double>();
            GetDistanceFromField(field);

        }

        ProcessLengthConstraint();
        ProcessCollision();
        ProcessBendingResistance();

        UpdatePosition(deltaTime);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>

    public void GetDistanceFromField(PerlinNoiseField field)
    {

        for (int i = 0; i < hemesh.Vertices.Count; i++)
        {
            var p = hemesh.Vertices[i].Position;
            double collisionDistance = field.ValueAt(p);
            collisionDistance = (collisionDistance + 1.0) / 2.0;
            collisionDistances.Add(collisionDistance * collisionDistanceBase);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public double GetCollisionDistanceValue(int index)
    {
        int last = collisionDistances.Count - 1;
        return index > last ? collisionDistances[last] : collisionDistances[index];

    }
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>

    public void SplitLongEdges()
    {
        int edgeCount = hemesh.Halfedges.Count;
        for (int k = 0; k < edgeCount; k += 2)
        {
            var edge = hemesh.Halfedges[k];
            int v0 =edge.Start.Index;
            int v1 = edge.End.Index;

            double r0 = GetCollisionDistanceValue(v0);
            double r1 = GetCollisionDistanceValue(v1);

            double collisionDistance = r0 + r1;
            if (hemesh.Vertices.Count < maxVertexCount && edge.GetLength() > 0.99 * collisionDistance)
            {
                SplitEdge(k);
                //hemesh.SplitEdge(edge);
            }
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="edgeIndex"></param>
    /// <returns></returns>
    public void SplitEdge(int edgeIndex)
    {
        var edge = hemesh.Halfedges[edgeIndex];
        var v0 = edge.Start;
        var v1 = edge.End;

     
        var newEdge = hemesh.SplitEdge(edge);
        var vn = 0.5 * ( v1.Position+v0.Position);

        hemesh.Vertices[hemesh.Vertices.Count - 1].Position = vn;

        hemesh.SplitFace(edge.Previous, newEdge);

        var e1 = edgeIndex % 2 == 0 ? hemesh.Halfedges[edgeIndex + 1] : hemesh.Halfedges[edgeIndex - 1];

        hemesh.SplitFace(e1, e1.Next.Next);
    }
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public void ProcessCollision()
    {
        int vertCount = hemesh.Vertices.Count;
        for (int i = 0; i < vertCount; i++)
        {
            for (int j = i + 1; j < vertCount; j++)
            {
                var move = hemesh.Vertices[j].Position - hemesh.Vertices[i].Position;
                double currentDistance = move.Length;
                var cdi = GetCollisionDistanceValue(i);
                var cdj = GetCollisionDistanceValue(j);

                double collisionDistance = cdi + cdj;

                if (currentDistance > collisionDistance) continue;
                move *= 0.5*(currentDistance - collisionDistance) / currentDistance;
                totalWeightedMoves[i] += move * collisionWeight * (cdi / collisionDistance);
                totalWeightedMoves[j] -= move * collisionWeight * (cdj / collisionDistance);
                totalWeight[i] += collisionWeight;
                totalWeight[j] += collisionWeight;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public void ProcessLengthConstraint()
    {
        int edgeCount = hemesh.Halfedges.Count;

        for (int k = 0; k < edgeCount; k += 2)
        {
            var halfedge = hemesh.Halfedges[k];
            var vi = halfedge.Start;
            var vj = halfedge.Next.Start;
            var d = vj.Position - vi.Position;

            int i = vi.Index;
            int j = vj.Index;

            double di = GetCollisionDistanceValue(i);
            double dj = GetCollisionDistanceValue(j);

            double distance = di + dj;

            if (d.Length > distance * 0.99)
            {
                var move = edgeLengthWeight * d;
                totalWeightedMoves[i] += move * (di / distance);
                totalWeightedMoves[j] -= move * (dj / distance);
                totalWeight[i] += edgeLengthWeight;
                totalWeight[j] += edgeLengthWeight;
            }
        }
    }

    public void ProcessBendingResistance()
    {
        int edgeCount = hemesh.Halfedges.Count;
        for (int k = 0; k < edgeCount; k += 2)
        {
            if (hemesh.Halfedges[k].Face==null || hemesh.Halfedges[k + 1].Face==null) continue;

            var vi = hemesh.Halfedges[k].Start;
            var vj = hemesh.Halfedges[k + 1].Start;
            var vip = hemesh.Halfedges[k].Previous.Start;
            var vjp = hemesh.Halfedges[k + 1].Previous.Start;

            Vec3d np = Vec3d.Cross(vj.Position - vi.Position, vip.Position - vi.Position);
            Vec3d nq = Vec3d.Cross(vjp.Position - vi.Position, vj.Position - vi.Position);

            var normal = np + nq;
            normal.Unitize();

            var origin = (vi.Position + vj.Position + vip.Position + vjp.Position) / 4.0;

            var plane = new Plane((Vector3)normal, (Vector3)origin);

            totalWeightedMoves[vi.Index] += (plane.ClosestPointOnPlane((Vector3)vi.Position) - vi.Position) * bendingResistance;
            totalWeightedMoves[vj.Index] += (plane.ClosestPointOnPlane((Vector3)vj.Position) - vj.Position) * bendingResistance;
            totalWeightedMoves[vip.Index] += (plane.ClosestPointOnPlane((Vector3)vip.Position) - vip.Position) * bendingResistance;
            totalWeightedMoves[vjp.Index] += (plane.ClosestPointOnPlane((Vector3)vjp.Position) - vjp.Position) * bendingResistance;
            totalWeight[vi.Index] += bendingResistance;
            totalWeight[vj.Index] += bendingResistance;
            totalWeight[vip.Index] += bendingResistance;
            totalWeight[vjp.Index] += bendingResistance;

        }
    }

    public void ProcessBoundCollision()
    {
        if (bound != null)
        {
            for (int i = 0; i < hemesh.Vertices.Count; i++)
            {
                var v = hemesh.Vertices[i];
                var p =(Vector3) v.Position;
                if (!bound.IsPointInside(p))
                {
                    var pb = bound.ClosestPoint(p);

                    totalWeightedMoves[i] += (Vec3d)(pb - p) * boundWeight;
                    totalWeight[i] += boundWeight;
                }
            }
        }
    }

    public void ProcessSmooth()
    {
        for (int i = 0; i < hemesh.Vertices.Count; i++)
        {
            var v = hemesh.Vertices[i];

            Vec3d center = Vec3d.Zero;
            int count = 0;

            foreach (var vn in v.ConnectedVertices)
            {
                center += vn.Position;
                count++;
            }

            if (count > 0)
            {
                center /= count;
                var move = center - v.Position;
               
               totalWeightedMoves[i]+=move.Unit *smoothWeight;
                totalWeight[i] += smoothWeight;
            }
        }
    }



    public void UpdatePosition(double deltaTime)
    {
        for (int i = 0; i < hemesh.Vertices.Count; i++)
        {
            if (totalWeight[i] == 0) continue;
            var move = totalWeightedMoves[i] / totalWeight[i];
            hemesh.Vertices[i].Position += deltaTime * move;
        }
    }
}




