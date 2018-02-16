using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths.Shapes;
using EnkiBye.Maths;



public class FractureMesh : MonoBehaviour
{
    const float gizmoSphereSize = 0.02f;
    private static readonly Color[] MeshColors = { Color.blue, Color.red, Color.yellow, Color.green, Color.gray, Color.magenta, Color.cyan, Color.black, Color.white };


    public List<Vector3> points = new List<Vector3>();

    public List<List<Vector3>> fracturePoints = new List<List<Vector3>>();

    public List<GameObject> fragments = new List<GameObject>();

    public Voronoi voronoi;

    public MeshFilter victim;

    MeshCreator3D mc3D;


    [Header("Gizmos")]
    public bool gizmosSelected = true;

    [Range(-1, 5)]
    public int gizmoPolyIndex = -1;
    public bool wires;
    public bool showIndices = false;
    public bool delayedStart = false;
    public bool addRigidbody = true;
    public bool explosion = false;
    public float explosionForce = 10;

    public FractureMesh()
    {
        Init();
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        mc3D = new MeshCreator3D();
    }

    IEnumerator BuildMeshes()
    {
        yield return null;


        for (int i = 0; i < fracturePoints.Count; i++)
        {
            if (i != gizmoPolyIndex && gizmoPolyIndex != -1) continue;
            Polygon3D poly = new Polygon3D(fracturePoints[i][0], fracturePoints[i][1], fracturePoints[i][2]);


            //Polygon3D poly = new Polygon3D(fracturePoints[i]);
            MeshFilter filt = mc3D.createMesh(poly);
            for (int j = 3; j < fracturePoints[i].Count; j++)
            {
                while (!Input.GetMouseButtonUp(1)) yield return null;

                Debug.Log("poly " + i + " : add point (" + fracturePoints[i][j].x + ", " + fracturePoints[i][j].y + ", " + fracturePoints[i][j].z + ")  number of vertices : " + poly.points.Count + "/" + fracturePoints[i].Count);
                poly.addPoint(fracturePoints[i][j]);


                mc3D.updateMesh(poly, filt);

                yield return null;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyFracture();
        }

    }

    public void ApplyFracture()
    {

        Debug.Log("FRACTURE");

        Voronoi();
        Fracture(victim, voronoi);
        ReplaceMesh();
        AddColliders();
        if (addRigidbody) AddRigidbody();
        if (explosion) ExplosionForce();
    }

    public void Voronoi()
    {
        voronoi = new Voronoi(points.ToArray());
    }


    public void Fracture()
    {
        Fracture(victim, voronoi);
    }

    public void Fracture(MeshFilter meshFilter, Voronoi voronoi)
    {
        List<List<Vector3>> polygons = new List<List<Vector3>>();
        for (int i = 0; i < voronoi.polygons.Count; i++)
        {
            polygons.Add(new List<Vector3>());
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3 objPos = meshFilter.transform.position;
        Quaternion objRot = meshFilter.transform.rotation;
        Vector3 objScale = meshFilter.transform.lossyScale;

        int[] originalsTriangles = mesh.GetTriangles(0);

        Plane plane = new Plane(voronoi.delaunay.points[0], voronoi.delaunay.points[1], voronoi.delaunay.points[2]);

        int t1, t2, t3; // the triangles indices
        Vector3[] trianglesPoints = new Vector3[3];
        Vector3[] scaledPoints = new Vector3[3];
        Vector3[] projectedPoints = new Vector3[3];
        int polygonIndex;

        for (int i = 0; i < originalsTriangles.Length; i += 3) // for each triangle
        {
            t1 = originalsTriangles[i];
            t2 = originalsTriangles[i + 1];
            t3 = originalsTriangles[i + 2];

            scaledPoints[0] = (mesh.vertices[t1] + objPos);
            scaledPoints[0] = new Vector3(mesh.vertices[t1].x * objScale.x, mesh.vertices[t1].y * objScale.y, mesh.vertices[t1].z * objScale.z) + objPos;
            scaledPoints[1] = (mesh.vertices[t2] + objPos);
            scaledPoints[1] = new Vector3(mesh.vertices[t2].x * objScale.x, mesh.vertices[t2].y * objScale.y, mesh.vertices[t2].z * objScale.z) + objPos;
            scaledPoints[2] = (mesh.vertices[t3] + objPos);
            scaledPoints[2] = new Vector3(mesh.vertices[t3].x * objScale.x, mesh.vertices[t3].y * objScale.y, mesh.vertices[t3].z * objScale.z) + objPos;

            trianglesPoints[0] = scaledPoints[0].RotateAroundPivot(objPos, objRot);
            trianglesPoints[1] = scaledPoints[1].RotateAroundPivot(objPos, objRot);
            trianglesPoints[2] = scaledPoints[2].RotateAroundPivot(objPos, objRot);

            //trianglesPoints[0] = (mesh.vertices[t1] + objPos).RotateAroundPivot(objPos, objRot);
            //trianglesPoints[1] = (mesh.vertices[t2] + objPos).RotateAroundPivot(objPos, objRot);
            //trianglesPoints[2] = (mesh.vertices[t3] + objPos).RotateAroundPivot(objPos, objRot);


            projectedPoints[0] = plane.ClosestPointOnPlane(trianglesPoints[0]);
            projectedPoints[1] = plane.ClosestPointOnPlane(trianglesPoints[1]);
            projectedPoints[2] = plane.ClosestPointOnPlane(trianglesPoints[2]);

            Triangle tri = new Triangle(projectedPoints[0], projectedPoints[1], projectedPoints[2]);

            for (int j = 0; j < 3; j++) // for each point of the triangle
            {

                polygonIndex = voronoi.pointContainer(projectedPoints[j]); // determine in wich polygon it is
                Polygon poly = voronoi.polygons[polygonIndex];
                if (!polygons[polygonIndex].Contains(trianglesPoints[j])) polygons[polygonIndex].Add(trianglesPoints[j]); // add the original point to the corresponding polygon.
                int nextPointIndex = (j != 2) ? j + 1 : 0;

                int nextPointPolyIndex = voronoi.pointContainer(projectedPoints[nextPointIndex]);
                if (nextPointPolyIndex == polygonIndex) continue; // Si le point suivant est dans le même polygon, on ne fait rien.
                //Sinon, il faut trouver les polygons qui sont entre les deux.

                for (int e = 0; e < poly.segments.Length; e++) // for each segment of the polygon
                {
                    bool onSegment;
                    Vector3 intersect = poly.segments[e].intersection(new Segment2D(projectedPoints[j], projectedPoints[nextPointIndex]), out onSegment);
                    if (!onSegment) continue;
                    bool projectionFailed = false;
                    Vector3 projectedIntersection = MathsTool3D.projectPointOnPlane(new Ray(intersect, plane.normal), new Plane(trianglesPoints[0], trianglesPoints[1], trianglesPoints[2]), out projectionFailed);
                    if (projectionFailed) continue;
                    polygons[polygonIndex].Add(projectedIntersection);
                    for (int p = 0; p < voronoi.polygons.Count; p++) // for each other polygon
                    {
                        if (voronoi.polygons[p] == poly) continue;
                        //if (voronoi.polygons[p].HasSegment(poly.segments[e])) polygons[p].Add(projectedIntersection);
                    }

                }// for each segment of the polygon

            }// for each point of the triangle

            for (int o = 0; o < voronoi.innerPoints.Count; o++)  // for each point of the voronoi
            {
                if (!tri.ContainPoint(voronoi.innerPoints[o])) continue;
                bool projectionFailed = false;
                Vector3 projectedVoronoi = MathsTool3D.projectPointOnPlane(new Ray(voronoi.innerPoints[o], plane.normal), new Plane(trianglesPoints[0], trianglesPoints[1], trianglesPoints[2]), out projectionFailed);
                if (projectionFailed) continue;
                for (int v = 0; v < voronoi.polygons.Count; v++) // project it on the mesh
                {
                    if (voronoi.polygons[v].HasVertice(voronoi.innerPoints[o])) polygons[v].Add(projectedVoronoi);
                }
            }

        } // for each triangle


        fracturePoints.Clear();
        bool skip = false;
        for (int i = 0; i < polygons.Count; i++)
        {
            fracturePoints.Add(new List<Vector3>());
            for (int j = 0; j < polygons[i].Count; j++)
            {
                skip = false;
                for (int k = j+1; k < polygons[i].Count; k++)
                {
                    if (Vector3.Distance(polygons[i][j], polygons[i][k]) < 0.001f)
                    {
                        skip = true;  // on veut éviter les doublons de points
                        break;
                    }
                }
                if (skip) continue;
                //Debug.Log("point " + i + ",  " + j + " = " + polygons[i][j]);
                fracturePoints[i].Add(polygons[i][j]);
            }
        }


    }//Fracture

    public void ReplaceMesh()
    {
        if (delayedStart)
        {
            StartCoroutine(BuildMeshes());
        }
        else
        {
            for (int i = 0; i < fracturePoints.Count; i++)
            {
                if (i != gizmoPolyIndex && gizmoPolyIndex != -1) continue;
                Debug.Log("build mesh : " + i);
                Polygon3D poly = new Polygon3D(fracturePoints[i]);
                MeshFilter filt = mc3D.createMesh(poly);
                fragments.Add(filt.gameObject);
            }
        }

        victim.gameObject.SetActive(false);

    }

    public void AddColliders()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            MeshCollider mCol = fragments[i].AddComponent<MeshCollider>();
            mCol.sharedMesh = fragments[i].GetComponent<MeshFilter>().mesh;
            mCol.convex = true;
        }
    }

    public void AddRigidbody()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            fragments[i].AddComponent<Rigidbody>();
        }
    }

    public void ExplosionForce()
    {
        Vector3 explosionPos = Vector3.zero; ;

        for (int i = 0; i < fragments.Count; i++)
        {
            explosionPos += fragments[i].transform.position / fragments.Count;
        }

        for (int i = 0; i < fragments.Count; i++)
        {
            Rigidbody rb = fragments[i].GetComponent<Rigidbody>();
            if (rb == null)
                rb = fragments[i].AddComponent<Rigidbody>();

            rb.useGravity = false;

            rb.AddExplosionForce(explosionForce, explosionPos, explosionForce);
        }
    }

    public void ExplosionForce(Vector3 pos)
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            Rigidbody rb = fragments[i].GetComponent<Rigidbody>();
            if (rb == null)
                rb = fragments[i].AddComponent<Rigidbody>();

            rb.useGravity = false;

            rb.AddExplosionForce(explosionForce * 50, pos, explosionForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (gizmosSelected) DrawGizmos();
    }

    private void OnDrawGizmos()
    {
        if (!gizmosSelected) DrawGizmos();

        UpdatePoints();
    }

    private void DrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], gizmoSphereSize);
        }

        if (voronoi == null) return;
        for (int i = 0; i < voronoi.polygons.Count; i++)
        {
            voronoi.polygons[i].Draw();
        }

        //Gizmos.color = Color.blue;
        for (int i = 0; i < fracturePoints.Count; i++)
        {
            if (i != gizmoPolyIndex && gizmoPolyIndex != -1) continue;

            Gizmos.color = GetFragmentColor(i);

            //new Polygon3D(fracturePoints[i]).Draw();
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);

            for (int j = 0; j < fracturePoints[i].Count; j++)
            {

                if (wires)
                {
                    for (int a = 0; a < fracturePoints[i].Count; a++)
                    {
                        Gizmos.DrawLine(fracturePoints[i][j], fracturePoints[i][a]);
                    }
                }
                else
                {
                    Gizmos.DrawCube(fracturePoints[i][j], Vector3.one * 0.025f);
                }
            }
        }

    }

    private void UpdatePoints()
    {
        if (transform.childCount < 3) return;

        bool changed = false;

        if (transform.childCount != points.Count)
        {
            changed = true;
            points = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++)
            {
                points.Add(transform.GetChild(i).position);
            }
        }


        Vector3 pos = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            pos = transform.GetChild(i).position;
            if(points[i] != pos)
            {
                points[i] = pos;
                changed = true;
            }
        }

        if (changed)
        {
            Voronoi();
            Fracture();
        }

    }


    public static Color GetFragmentColor(int index)
    {
        return MeshColors[index % MeshColors.Length];
    }

}
