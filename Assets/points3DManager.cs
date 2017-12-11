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
        newPoint = Random.insideUnitSphere * range;

        newPointTR.transform.position = newPoint;

    }

    public void addNewPoint()
    {
        convexHull.addPoint(newPoint);
        points.Add(newPoint);

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

        if (GUILayout.Button("CreateNewPoint"))
            CreateNewPoint();

        if (GUILayout.Button("addNewPoint"))
            addNewPoint();

        if (GUILayout.Button("change material"))
        {
            matIndex++;
            if (matIndex >= meshMats.Length) matIndex = 0;
            meshCreator.SetMaterial(meshMats[matIndex]);

        }

    }

}
