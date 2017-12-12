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

    [HideInInspector] public DrawLines linesTriangulation = new DrawLines();
    [HideInInspector] public DrawLines linesVoronoi = new DrawLines();

    MeshCreator meshesVoronoi = new MeshCreator();

    [HideInInspector]
    List<DisplayTriangle> meshes = new List<DisplayTriangle>();
    bool meshesEnabled = true;
    bool canFocus = false;

    bool moving = false;

    public float percentChanceToTurn = 0.5f;
    public float moveSpeed = 5f;


    public void OnButtonClick()
    {
        clear();

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
        movePoints();

        computeMousePos();

        updateTriangulation();

        getKeys();

    }

    public void getKeys()
    {
        if (Input.GetKeyUp(KeyCode.Keypad1))
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

        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            canFocus = !canFocus;
        }

        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            clear();
        }

        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            meshesVoronoi.clear();
            meshesVoronoi.ToggleEnable(!meshesVoronoi.enabled);
            if (meshesVoronoi.enabled) recreateVoronoiMeshes();
        }

        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            moving = !moving;
        }

        if (Input.GetKey(KeyCode.KeypadPlus)) moveSpeed = Mathf.Clamp(moveSpeed+=Time.deltaTime*10, 0.1f, 100);
        if (Input.GetKey(KeyCode.KeypadMinus)) moveSpeed = Mathf.Clamp(moveSpeed-=Time.deltaTime*10, 0.1f, 100);



    }

    public void updateTriangulation()
    {
        bool recalculate = false;
        for (int i = 0; i < points.Count; i++)
        {
            if (Vector2.Distance( points[i], (Vector2)pointsTr[i].position) > 0.01f) recalculate = true;
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
                if (meshes[i].triangle.ContainPoint(mousePos) && canFocus)
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

    public void movePoints()
    {
        if (!moving) return;
        for (int i = 0; i < pointsTr.Count; i++)
        {
            pointsTr[i].transform.position += pointsTr[i].forward * Time.deltaTime * moveSpeed;
            if(Random.Range(0f, 100f) <= percentChanceToTurn)
                pointsTr[i].Rotate(Random.Range(-360, 360), 0, 0);

            if (pointsTr[i].position.magnitude > area) pointsTr[i].LookAt(Vector3.zero);
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
                pointsTr[pointsTr.Count - 1].LookAt(Vector3.zero);
                pointsTr[pointsTr.Count - 1].Rotate(Random.Range(-360, 360), 0, 0);
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

        if(linesVoronoi.lines.Count != voronoi.numberOfEdges)
        {
            linesVoronoi.CleanLinesList();
            for (int i = 0; i < voronoi.polygons.Count ; i++)
            {
                for (int j = 0; j < voronoi.polygons[i].segments.Length; j++)
                {
                    linesVoronoi.DrawLine_LR(voronoi.polygons[i].segments[j].points, true, Color.green);
                }
            }
        }
        else
        {
            int n = 0;
            for (int i = 0; i < voronoi.polygons.Count; i++)
            {
                for (int j = 0; j < voronoi.polygons[i].segments.Length; j++)
                {
                    linesVoronoi.DrawLine_LR(voronoi.polygons[i].segments[j].points, linesVoronoi.lines[n], true, Color.green);
                    n++;
                }
            }
        }



        SimpleCamControll.get().updateSizes(this);

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

        //Gizmos.DrawWireSphere(Vector3.zero, area);

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


        Gizmos.color = Color.red;
        voronoi.Draw();

        //if (voronoi != null && voronoi.points != null)
        //{
        //    for (int i = 0; i < voronoi.points.Count; i++)
        //    {
        //        Gizmos.color = Color.green;
        //        Gizmos.DrawSphere(voronoi.points[i], 10.5f);
        //    }
        //    for (int i = 0; i < voronoi.segments.Count; i++)
        //    {
        //        voronoi.segments[i].Draw();
        //    }
        //}

    }

    private void clear()
    {

        points = new List<Vector2>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            points.Add(new Vector2(Random.Range(-area, area), Random.Range(-area, area)));
        }

        points.sortX();
        convexHull = Jarvis.GetConvexHull(points);
        triangulation = null;
        OnTriangulate();
        if (Application.isPlaying)
        {
            RecreatePoints();
        }
    }

    private void recreateVoronoiMeshes()
    {
        meshesVoronoi.clear();
        for (int i = 0; i < voronoi.polygons.Count; i++)
        {
            meshesVoronoi.createMesh(voronoi.polygons[i], new Vector3(0, 0, -1), Random.ColorHSV());
        }
    }


    public void OnGUI()
    {


        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperLeft;

        GUILayout.Label("Number of points : " + points.Count, labelStyle);



        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.alignment = TextAnchor.UpperLeft;

        if (GUILayout.Button("Toggle triangulation", style))
        {
            linesTriangulation.toggleEnable();
            meshesEnabled = !meshesEnabled;
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].toggleEnable(meshesEnabled);
            }
        }

        if (GUILayout.Button("Toggle Voronoi", style))
        {
            linesVoronoi.toggleEnable();
        }

        if (GUILayout.Button("Toggle Triangles Focus", style))
        {
            canFocus = !canFocus;
        }

        if (GUILayout.Button("Toggle Colored poly", style))
        {
            meshesVoronoi.clear();
            meshesVoronoi.ToggleEnable(!meshesVoronoi.enabled);
            if (meshesVoronoi.enabled) recreateVoronoiMeshes();
        }

        if (GUILayout.Button("Reset", style))
        {
            clear();
        }

        if (GUILayout.Button("Moving", style))
        {
            moving = !moving;
        }


        GUILayout.Button(new GUIContent("HELP", "\n\n<b>Controls</b> : \n" +
        "<b>Middle click + Move mouse</b> : move\n" +
        "<b>Left click on point</b> : Drag&drop this point\n" +
        "<b>Right click on point</b> : Delete this point\n" +
        "<b>Right click anywhere</b> : Add point"));

        Vector2 mousePos = Event.current.mousePosition;

        GUIStyle HelpStyle = new GUIStyle(GUI.skin.label);
        HelpStyle.alignment = TextAnchor.UpperLeft;

        Rect tooltipPos = new Rect(mousePos, new Vector2(500, 500));
        GUI.Label(tooltipPos, GUI.tooltip, HelpStyle);
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