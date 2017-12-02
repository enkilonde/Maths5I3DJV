using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths.Shapes;
using EnkiBye.Maths;

public class MeshCreator
{

    public bool enabled = true;

    public List<MeshFilter> meshes = new List<MeshFilter>(); 

    public void updateMesh(Polygon poly, MeshFilter filt)
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

    //2D
    public void createMesh(Polygon poly, Vector3 offset = default(Vector3), Color color = default(Color))
    {
        GameObject obj = new GameObject("poly");
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        rend.material = new Material(Shader.Find("Diffuse"));
        MeshFilter filt = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filt.mesh = mesh;
        meshes.Add(filt);

        obj.transform.localScale = new Vector3(1, 1, -1);
        obj.transform.position = (Vector3)poly.barycenter + offset;

        updateMesh(poly, filt);
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
