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

    private void Start()
    {
        convexHull = MathsTool3D.getConvexHull(points);

        meshCreator = new MeshCreator3D();
        meshCreator.createMesh(convexHull);



    }




    public void CreateNewPoint()
    {
        newPoint = Random.insideUnitSphere * range;
    }

    public void addNewPoint()
    {
        convexHull.addPoint(newPoint);
        points.Add(newPoint);

        meshCreator.updateMesh(convexHull, meshCreator.meshes[0]);
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

}
