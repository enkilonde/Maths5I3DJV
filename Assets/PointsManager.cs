using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using EnkiBye.Maths;
using EnkiBye.Maths.Shapes;

public class PointsManager : MonoBehaviour
{
    static PointsManager instance;
    public static PointsManager get()
    {
        if(instance == null)
        {
            instance = GameObject.FindObjectOfType<PointsManager>();
        }

        return instance;
    }
    public static bool next = false;

    private Transform dragedPoint = null;

    [Header("Parameters")]
    public int numberOfPoints = 10;
    public float area = 10;

    public float pointSize = 0.1f;
    public Gradient pointColor;

    [Header("Datas")]
    [HideInInspector]
    public List<Vector2> points;

    //[HideInInspector]
    public List<Transform> pointsTr;

    [HideInInspector]
    public Polygon convexHull;

    [HideInInspector]
    public Triangulation triangulation;

    public Voronoi voronoi;

    private DrawLines linesTriangulation = new DrawLines();
    private DrawLines linesVoronoi = new DrawLines();

    [HideInInspector]
    List<DisplayTriangle> meshes = new List<DisplayTriangle>();
    bool meshesEnabled = true;

    public void OnButtonClick()
    {
        //Debug.Log("clic");
        points.sortX();
        convexHull = Jarvis.GetConvexHull(points);
        triangulation = null;
        //triangulation = new Triangulation(points);
        OnTriangulate();
        if (Application.isPlaying)
        {
            RecreatePoints();
        }

    }

    public void OnTriangulate()
    {
        triangulation = new Triangulation(points);
        voronoi = new Voronoi(triangulation);
    }

    public void OnUpdateHull()
    {
        convexHull = Jarvis.GetConvexHull(points);
    }

    public void CheckDelaunayValidity()
    {
        bool validity = true;
        for (int i = 0; i < triangulation.triangles.Count; i++)
        {
            for (int j = 0; j < triangulation.points.Count; j++)
            {
                if(!triangulation.triangles[i].HasVertice(triangulation.points[j]) && triangulation.triangles[i].isinCircumscribed(triangulation.points[j]))
                {
                    Debug.Log("error on triangle " + i + "   and on point " + j);
                    validity = false;
                    break;
                }
            }
            if (!validity) break;
        }

        Debug.Log("Triangulation is " + validity);

    }

    private void Awake()
    {
        RecreatePoints();
    }

    private void Update()
    {
        updateTriangulation();

        computeMousePos();

        if(Input.GetKeyUp(KeyCode.Keypad1))
        {
            linesTriangulation.toggleEnable();
            meshesEnabled = !meshesEnabled;
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].toggleEnable(meshesEnabled);
            }

        }

        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            linesVoronoi.toggleEnable();
        }


    }

    public void updateTriangulation()
    {
        bool recalculate = false;
        for (int i = 0; i < points.Count; i++)
        {
            if (Vector2.Distance( points[i], (Vector2)pointsTr[i].position) > 0.1f) recalculate = true;
            points[i] = pointsTr[i].position;
        }
        if (recalculate) 
        {
            points.sortX();
            convexHull = Jarvis.GetConvexHull(points);
            triangulation = new Triangulation(points);
            voronoi = new Voronoi(triangulation);
            createMeshes();
        }

    }

    public void computeMousePos()
    {
        Vector2 mousePos = MouseUtils.CursorWorldPosDepth(0);



        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(1)) && dragedPoint == null)
        {

            dragedPoint = GetNearestPoint(mousePos, pointsTr[0].localScale.x); // get nearest point

            for (int i = 0; i < meshes.Count; i++) meshes[i].unFocus(); // unFocus all meshes

            if(Input.GetMouseButtonUp(1))
            {
                if(dragedPoint == null)
                {
                    addPoint(new Vector2(mousePos.x, mousePos.y));
                }
                else
                {
                    RemovePoint(dragedPoint);
                }
                dragedPoint = null;
                return;
            }
        }

        if (dragedPoint != null)
        {
            dragedPoint.position = new Vector3(mousePos.x, mousePos.y, dragedPoint.position.z);
        }
        else
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].triangle.ContainPoint(mousePos))
                {
                    meshes[i].focus();
                }
                else
                {
                    meshes[i].unFocus();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && dragedPoint != null)
        {
            RecreatePoints();
            dragedPoint = null;
        }
    }

    public void RecreatePoints()
    {
        if (points != null)
        {
            for (int i = 0; i < pointsTr.Count; i++)
            {
                Destroy(pointsTr[i].gameObject);
            }
            pointsTr = new List<Transform>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                pointsTr.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere).transform);
                pointsTr[pointsTr.Count - 1].position = points[i];
            }
            triangulation = new Triangulation(points);
            voronoi = new Voronoi(triangulation);
            createMeshes();

            
        }
    }

    public void createMeshes()
    {
        if(meshes.Count != triangulation.triangles.Count)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].clean();
            }
            meshes = new List<DisplayTriangle>();
            meshes.Clear();
            linesTriangulation.CleanLinesList();

            for (int i = 0; i < triangulation.triangles.Count; i++)
            {
                meshes.Add(new DisplayTriangle(triangulation.triangles[i]));
                meshes[meshes.Count - 1].toggleEnable(meshesEnabled);
                linesTriangulation.DrawLine_LR(triangulation.triangles[i].points, true);
            }
        }
        else
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].applyTriangle(triangulation.triangles[i]);
                linesTriangulation.DrawLine_LR(triangulation.triangles[i].points, linesTriangulation.lines[i], true);
            }
        }

        if(linesVoronoi.lines.Count != voronoi.segments.Count)
        {
            linesVoronoi.CleanLinesList();
            for (int i = 0; i < voronoi.segments.Count; i++)
            {
                linesVoronoi.DrawLine_LR(voronoi.segments[i].points, true, Color.green);
            }
        }
        else
        {
            for (int i = 0; i < voronoi.segments.Count; i++)
            {
                linesVoronoi.DrawLine_LR(voronoi.segments[i].points, linesVoronoi.lines[i], true, Color.green);
            }
        }



        SimpleCamControll.get().updateSizes();

    }//createMeshes

    public void addPoint(Vector2 pos)
    {
        points.Add(pos);

        points.sortX();
        convexHull = Jarvis.GetConvexHull(points);
        triangulation = new Triangulation(points);
        voronoi = new Voronoi(triangulation);
        RecreatePoints();
    }

    public void RemovePoint(Transform transf)
    {
        points.RemoveAt(pointsTr.findIndex(transf));
        points.sortX();
        convexHull = Jarvis.GetConvexHull(points);
        triangulation = new Triangulation(points);
        voronoi = new Voronoi(triangulation);
        RecreatePoints();
    }

    public Transform GetNearestPoint(Vector2 pos, float minDist = Mathf.Infinity)
    {

        int nearestPointIndex = -1;
        for (int i = 0; i < pointsTr.Count; i++)
        {
            float dist = Vector2.Distance(pos, pointsTr[i].position);
            if(dist <= minDist)
            {
                minDist = dist;
                nearestPointIndex = i;
            }
        }
        return nearestPointIndex >= 0 ? pointsTr[nearestPointIndex] : null;
    }

    private void OnDrawGizmos()
    {

        if (points.Count <= 0) return;

        //points.bubbleSortX();

        Gizmos.color = Color.red;
        if (convexHull != null) convexHull.Draw();


        for (int i = 0; i < points.Count; i++)
        {
            float color = (float)i / (float)points.Count;
            Gizmos.color = pointColor.Evaluate(color);
            Gizmos.DrawSphere(points[i], pointSize);
        }

        Gizmos.color = Color.white;

        if(triangulation != null)
            triangulation.Draw();


        if(voronoi != null && voronoi.points != null)
        {
            for (int i = 0; i < voronoi.points.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(voronoi.points[i], 0.5f);
            }
            for (int i = 0; i < voronoi.segments.Count; i++)
            {
                voronoi.segments[i].Draw();
            }
        }

    }

}





public class DisplayTriangle
{
    public Triangle triangle;
    public MeshFilter meshFilter;

    public Transform circle;

    bool enabled = true;

    public bool focused = false;

    public DisplayTriangle(Triangle tri)
    {
        triangle = tri;
        Vector2 center = tri.barycenter;
        Transform tr = new GameObject("triangle").transform;
        MeshRenderer mr = tr.gameObject.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Diffuse"));

        circle = new GameObject("Circle").transform;
        SpriteRenderer circleRend = circle.gameObject.AddComponent<SpriteRenderer>();
        circleRend.sprite = Resources.Load<Sprite>("Circle2");
        circleRend.color = Color.gray;
        circle.gameObject.SetActive(focused);
        circle.position = (Vector3)triangle.hortocenter + new Vector3(0, 0, -2);
        circle.localScale = Vector3.one * (triangle.hortocenter - triangle.a).magnitude * 2;

        Mesh m = new Mesh();

        List<Vector3> vertices = new List<Vector3>(3);
        vertices.Add(tri.a - center);
        vertices.Add(tri.b - center);
        vertices.Add(tri.c - center);
        m.SetVertices(vertices);
        int[] tris = new int[3] { 2, 1, 0 };
        m.SetTriangles(tris, 0);

        meshFilter = tr.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = m;
        tr.transform.position = center;




    }

    public void applyTriangle(Triangle tri)
    {
        triangle = tri;
        Vector2 center = tri.barycenter;

        List<Vector3> vertices = new List<Vector3>(3);
        vertices.Add(tri.a - center);
        vertices.Add(tri.b - center);
        vertices.Add(tri.c - center);

        meshFilter.mesh.SetVertices(vertices);
        int[] tris = new int[3] { 2, 1, 0 };
        meshFilter.mesh.SetTriangles(tris, 0);

        meshFilter.transform.position = center;

        circle.position = (Vector3)triangle.hortocenter + new Vector3(0, 0, -2);
        circle.localScale = Vector3.one * (triangle.hortocenter - triangle.a).magnitude * 2;
    }

    public void focus()
    {
        if (focused) return;
        focused = true;

        circle.gameObject.SetActive(true && enabled);
        meshFilter.GetComponent<MeshRenderer>().material.color = Color.red;

    }

    public void unFocus()
    {
        if (!focused) return;
        focused = false;
        circle.gameObject.SetActive(false);
        meshFilter.GetComponent<MeshRenderer>().material.color = Color.white;


    }

    public void clean()
    {
        GameObject.Destroy(meshFilter.gameObject);
        GameObject.Destroy(circle.gameObject);
    }

    public void toggleEnable(bool flag)
    {
        enabled = flag;
        meshFilter.gameObject.SetActive(enabled);
        circle.gameObject.SetActive(enabled && focused);
    }
}


//https://www.researchgate.net/post/How_can_I_perform_Delaunay_Triangulation_algorithm_in_C