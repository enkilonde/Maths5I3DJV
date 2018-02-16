using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths.Shapes;
using EnkiBye.Maths;

public class MeshCreator
{

    public bool enabled = true;

    public List<MeshFilter> meshes = new List<MeshFilter>(); 

    public void UpdateMesh(Polygon poly, MeshFilter filt)
    {
        List<Vector2> points = new List<Vector2>(poly.points);

        Vector3 center = poly.barycenter;
        points.Insert(0, Vector2.zero);


        List<Vector3> MeshPoints = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 1; i < points.Count; i++)
        {
            points[i] -= (Vector2)center;
            triangles.Add(0);
            triangles.Add(i);

            triangles.Add((i + 1 >= points.Count ? 1 : i + 1));
        }

        filt.mesh.SetVertices(points.toVector3List());
        filt.mesh.SetTriangles(triangles, 0);

    }

    public void UpdateMesh(Polygon poly, MeshFilter filt, float depth)
    {
        List<Vector3> points = new List<Vector3>(poly.points.toVector3Array());

        int pointCount = points.Count;
        Vector3 center = poly.barycenter;

        

        for (int i = 0; i < pointCount; i++)
        {
            points[i] -= center;
            points.Add(points[i] + new Vector3(0, 0, depth));
        }

        Vector3 Center3D = new Vector3(0, 0, depth / 2);

        points.Insert(0, Vector3.zero);
        points.Insert(1, new Vector3(0, 0, depth));
        int listBegin = 2; // it is the number of point we added at the begening of the list, before the vertices, here, we added the center and the center of the extruded face

        List<int> triangles = new List<int>();

        for (int i = listBegin; i < pointCount + listBegin; i++)
        {
            int nextPoint = i + 1;
            if(nextPoint >= pointCount + listBegin)
            {
                nextPoint = listBegin;
            }
            //Il faut, pour chaque face, la flipper si elle ne regarde pas vers le centre du poly
            //centre du poly = centre du segment entre points[0] et points[1]



            Face triangle = new Face(points[0], points[nextPoint], points[i]);

            //face
            if (triangle.seePoint(Center3D))
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(nextPoint);
            }else
            {
                triangles.Add(0);
                triangles.Add(nextPoint);
                triangles.Add(i);
            }

            
            //top1
            triangle = new Face(points[i], points[nextPoint], points[i + pointCount]);
            if (triangle.seePoint(Center3D))
            {
                triangles.Add(i);
                triangles.Add(i + pointCount);
                triangles.Add(nextPoint);
            }else
            {
                triangles.Add(i);
                triangles.Add(nextPoint);
                triangles.Add(i + pointCount);
            }


            //top2
            triangle = new Face(points[nextPoint], points[nextPoint + pointCount], points[i + pointCount]);
            if (triangle.seePoint(Center3D))
            {
                triangles.Add(nextPoint);
                triangles.Add(i + pointCount);
                triangles.Add(nextPoint + pointCount);
            }else
            {
                triangles.Add(nextPoint);
                triangles.Add(nextPoint + pointCount);
                triangles.Add(i + pointCount);
            }


            //back
            triangle = new Face(points[1], points[i + pointCount], points[nextPoint +pointCount]);
            if (triangle.seePoint(Center3D))
            {
                triangles.Add(1);
                triangles.Add(nextPoint + pointCount);
                triangles.Add(i + pointCount);
            }
            else
            {
                triangles.Add(1);
                triangles.Add(i + pointCount);
                triangles.Add(nextPoint + pointCount);
            }

        }

        filt.mesh.SetVertices(points);
        filt.mesh.SetTriangles(triangles, 0);
        filt.mesh.RecalculateNormals();
    }

    //2D
    public void CreateMesh(Polygon poly, Vector3 offset = default(Vector3), Color color = default(Color))
    {
        GameObject obj = new GameObject("poly");
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        rend.material = new Material(Shader.Find("Standard"));
        MeshFilter filt = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filt.mesh = mesh;
        meshes.Add(filt);

        obj.transform.localScale = new Vector3(1, 1, -1);
        obj.transform.position = (Vector3)poly.barycenter + offset;

        UpdateMesh(poly, filt);
        rend.material.color = color;
        obj.SetActive(enabled);



    }


    //3D
    public void CreateMesh(Polygon poly, float depth, Vector3 offset = default(Vector3), Color color = default(Color))
    {
        GameObject obj = new GameObject("poly");
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        rend.material = new Material(Shader.Find("Standard"));
        MeshFilter filt = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filt.mesh = mesh;
        meshes.Add(filt);

        obj.transform.localScale = new Vector3(1, 1, -1);
        obj.transform.position = (Vector3)poly.barycenter + offset;

        UpdateMesh(poly, filt, depth);
        rend.material.color = color;
        obj.SetActive(enabled);



    }


    public void clear()
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            GameObject.Destroy(meshes[i].gameObject);
        }
        meshes.Clear();
    }


    public void ToggleEnable(bool flag)
    {
        enabled = flag;
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].gameObject.SetActive(enabled);
        }
    }


}
