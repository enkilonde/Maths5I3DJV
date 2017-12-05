using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnkiBye.Maths;
using EnkiBye.Maths.Shapes;

public class points3DManager : MonoBehaviour
{


    public List<Point> points = new List<Point>();



    private void Start()
    {
        Point p1 = new Point(new Vector3(0, 0, 1));
        Point p2 = new Point(new Vector3(1, 0, -1));
        Point p3 = new Point(new Vector3(-1, 0, -1));
        Point p4 = new Point(new Vector3(0, 1, 0));

        Face f1 = new Face(p1, p3, p2);
        Face f2 = new Face(p2, p3, p4);
        Face f3 = new Face(p3, p4, p1);
        Face f4 = new Face(p4, p1, p2);

        Face[] faces = new Face[4] { f1, f2, f3, f4 };

        Polygon3D poly = new Polygon3D(faces);

        MeshCreator3D M = new MeshCreator3D();

        M.createMesh(poly);

        for (int i = 0; i < points.Count; i++)
        {

        }


    }










}
