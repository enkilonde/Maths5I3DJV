using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines
{

    public static List<DrawLines> instances = new List<DrawLines>();

    public DrawLines()
    {
        instances.Add(this);
    }



    public static void resizeAll(float value = 1)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            instances[i].resize(value);
        }
    }

  





    bool enabled = true;
    public void toggleEnable()
    {
        setEnable(!enabled);
    }
    public void setEnable(bool flag)
    {
        enabled = flag;
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].enabled = enabled;
        }
    }

    public void resize(float value)
    {
        for (int j = 0; j < lines.Count; j++)
        {
            lines[j].widthMultiplier = value;
        }
    }

    public float depth = -0.1f;
    public List<LineRenderer> lines = new List<LineRenderer>();

    public void CleanLinesList()
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

    public LineRenderer DrawLine_LR(Vector3[] points, LineRenderer overideOld, bool loop = false, Color color = default(Color))
    {
        if (overideOld == null) return DrawLine_LR(points, loop);
        overideOld.positionCount = points.Length;
        overideOld.SetPositions(points);
        overideOld.loop = loop;
        overideOld.material.color = new Color(color.r, color.g, color.b, color == default(Color) ? 1 : color.a); 
        if (!lines.Contains(overideOld)) lines.Add(overideOld);
        overideOld.enabled = enabled;
        return overideOld;
    }

    public LineRenderer DrawLine_LR(Vector3[] points, bool loop = false, Color color = default(Color))
    {
        GameObject go = new GameObject("line renderer");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = 0.2f;
        lr.material = Resources.Load<Material>("Lines");
        lr.material.color = new Color(color.r, color.g, color.b, color == default(Color)?1:color.a);
        lr.positionCount = points.Length;
        lr.loop = loop;
        lr.SetPositions(points);
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;

        lines.Add(lr);
        lr.enabled = enabled;
        return lr;
    }


    public LineRenderer DrawLine_LR(Vector2[] points, LineRenderer overideOld, bool loop = false, Color color = default(Color))
    {
        if (overideOld == null) return DrawLine_LR(points, loop, color);
        return DrawLine_LR(AddDimention(points), overideOld, loop, color);
    }

    public LineRenderer DrawLine_LR(Vector2[] points, bool loop = false, Color color = default(Color))
    {
        return DrawLine_LR(AddDimention(points), loop, color);
    }



    public Vector3[] AddDimention(Vector2[] points)
    {
        Vector3[] output = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            output[i] = new Vector3(points[i].x, points[i].y, depth);
        }
        return output;
    } 

}
