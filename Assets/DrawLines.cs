using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines
{
    public static float depth = -0.1f;
    public static List<LineRenderer> lines = new List<LineRenderer>();


    public static void CleanLinesList()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if(lines[i] != null)
            {
                GameObject.Destroy(lines[i].gameObject);
            }
        }
        lines.Clear();
        lines = new List<LineRenderer>();
    }




    public static LineRenderer DrawLine_LR(Vector3[] points, LineRenderer overideOld, bool loop = false)
    {
        if (overideOld == null) return DrawLine_LR(points, loop);
        overideOld.positionCount = points.Length;
        overideOld.SetPositions(points);
        overideOld.loop = loop;
        if (!lines.Contains(overideOld)) lines.Add(overideOld);
        return overideOld;
    }

    public static LineRenderer DrawLine_LR(Vector3[] points, bool loop = false)
    {
        GameObject go = new GameObject("line renderer");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = 0.2f;
        lr.material = new Material(Shader.Find("Diffuse"));
        lr.material.color = Color.black;
        lr.positionCount = points.Length;
        lr.loop = loop;
        lr.SetPositions(points);
        lines.Add(lr);
        return lr;
    }


    public static LineRenderer DrawLine_LR(Vector2[] points, LineRenderer overideOld, bool loop = false)
    {
        if (overideOld == null) return DrawLine_LR(points);
        return DrawLine_LR(AddDimention(points), overideOld, loop);
    }

    public static LineRenderer DrawLine_LR(Vector2[] points, bool loop = false)
    {
        return DrawLine_LR(AddDimention(points), loop);
    }



    public static Vector3[] AddDimention(Vector2[] points)
    {
        Vector3[] output = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            output[i] = new Vector3(points[i].x, points[i].y, depth);
        }
        return output;
    } 

}
