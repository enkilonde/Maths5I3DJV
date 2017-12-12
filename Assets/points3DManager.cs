using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnkiBye.Maths;
using EnkiBye.Maths.Shapes;

public class points3DManager : MonoBehaviour
{
    public float range = 10;

    public List<Vector3> points = new List<Vector3>();

    public Vector3 newPoint;

    private Polygon3D convexHull;

    private MeshCreator3D meshCreator;

    public GameObject newPointTR;
    List<GameObject> pointsTR = new List<GameObject>();

    private int matIndex;
    public Material[] meshMats;


    private void Start()
    {
        convexHull = MathsTool3D.getConvexHull(points);

        meshCreator = new MeshCreator3D();
        meshCreator.createMesh(convexHull);


        for (int i = 0; i < points.Count; i++)
        {
            pointsTR.Add(Instantiate<GameObject>(newPointTR));
            pointsTR[i].transform.localScale *= 0.5f;
            pointsTR[i].transform.position = points[i];
            pointsTR[i].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.5f);
        }
        newPointTR.transform.position = newPoint;

    }




    public void CreateNewPoint()
    {
        MoveNewPoint(Random.insideUnitSphere * range);
    }

    public void MoveNewPoint(Vector3 pos)
    {
        newPoint = pos;

        newPointTR.transform.position = newPoint;
    }

    public void addNewPoint(Vector3 point)
    {
        MoveNewPoint(point);
        convexHull.addPoint(point);
        points.Add(point);

        meshCreator.updateMesh(convexHull, meshCreator.meshes[0]);

        pointsTR.Add(Instantiate<GameObject>(newPointTR));
        pointsTR[pointsTR.Count-1].transform.localScale *= 0.5f;
        pointsTR[pointsTR.Count - 1].transform.position = newPoint;
        pointsTR[pointsTR.Count - 1].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.5f);
    }


    private void OnDrawGizmosSelected()
    {
        float gizmoSize = 0.1f;

        Gizmos.DrawWireSphere(transform.position, range);


        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(newPoint, gizmoSize);

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], gizmoSize);
        }

    }


    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperLeft;

        GUILayout.Label("Number of points : " + points.Count, labelStyle);


        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.alignment = TextAnchor.UpperLeft;

        if (GUILayout.Button("set red point on front", style))
            MoveNewPoint(Camera.main.transform.position + Camera.main.transform.forward);

        if (GUILayout.Button("move red point", style))
            CreateNewPoint();

        if (GUILayout.Button("add red point", style))
            addNewPoint(newPoint);

        if (GUILayout.Button("add random point", style))
            addNewPoint(Random.insideUnitSphere * range);

        if (GUILayout.Button("change material", style))
        {
            matIndex++;
            if (matIndex >= meshMats.Length) matIndex = 0;
            meshCreator.SetMaterial(meshMats[matIndex]);
        }


        GUILayout.Button(new GUIContent("HELP", "\n\n<b>Controls</b> : \n" +
            "<b>ZQSD</b> : move\n" +
            "<b>A-E</b> : move UP-DOWN\n" +
            "<b>Right click + move mouse</b> : Look around\n" +
            "<b>Left click + move mouse</b> : Change light"));

        Vector2 mousePos = Event.current.mousePosition;

        GUIStyle HelpStyle = new GUIStyle(GUI.skin.label);
        HelpStyle.alignment = TextAnchor.UpperLeft;

        Rect tooltipPos = new Rect(mousePos, new Vector2(500, 500));
        GUI.Label(tooltipPos, GUI.tooltip, HelpStyle);

    }

}
