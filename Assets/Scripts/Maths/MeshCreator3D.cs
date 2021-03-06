﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnkiBye.Maths.Shapes;
using EnkiBye.Maths;



public class MeshCreator3D
{


    public bool enabled = true;

    public List<MeshFilter> meshes = new List<MeshFilter>();


    public void updateMesh(Polygon3D polygon, MeshFilter filt)
    {
        List<Vector3> points = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < polygon.faces.Length; i++)
        {
            points.Add(polygon.faces[i][0] - filt.transform.position);
            points.Add(polygon.faces[i][1] - filt.transform.position);
            points.Add(polygon.faces[i][2] - filt.transform.position);
            triangles.Add(3 * i + 0);
            triangles.Add(3 * i + 1);
            triangles.Add(3 * i + 2);
        }
        
        filt.mesh.SetVertices(points);

        filt.mesh.SetTriangles(triangles, 0);



        filt.mesh.RecalculateNormals();
        
    }


    public MeshFilter createMesh(Polygon3D polygon)
    {

        GameObject obj = new GameObject("poly");
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        rend.material = Resources.Load<Material>("Opaque");
        //rend.material.color = new Color(1, 1, 1, 0.8f);
        //rend.material.SetFloat("_Mode", 3.0f);
        //rend.material.color = new Color(1, 1, 1, 0.8f);

        MeshFilter filt = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filt.mesh = mesh;
        meshes.Add(filt);

        //obj.transform.localScale = new Vector3(1, 1, -1);
        obj.transform.position = polygon.barycenter;

        updateMesh(polygon, filt);
        obj.SetActive(enabled);





        return filt;
    }


    public void SetMaterial(Material mat)
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].GetComponent<Renderer>().material = mat;
        }
    }

}
