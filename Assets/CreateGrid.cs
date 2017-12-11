using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    public float ecart = 10;
    public float number = 100;

    public float size = 0.01f;
    public Gradient color;

    // Use this for initialization
    void Start ()
    {
        DrawLines lines = new DrawLines();
        for (float x = -number; x < number; x += ecart)
        {
            for (float y = -number; y < number; y += ecart)
            {
                lines.DrawLine_LR(new Vector3[] { new Vector3(x, -number, y), new Vector3(x, number, y) }, false, color.Evaluate(0));
                lines.DrawLine_LR(new Vector3[] { new Vector3(-number, x, y), new Vector3(number, x, y) }, false, color.Evaluate(0));
                lines.DrawLine_LR(new Vector3[] { new Vector3(x, y, -number), new Vector3(x, y, number) }, false, color.Evaluate(0));

            }
        }



        for (int i = 0; i < lines.lines.Count; i++)
        {
            lines.lines[i].widthMultiplier = size;
            lines.lines[i].colorGradient = color;
            lines.lines[i].transform.SetParent(transform);
        }


	}
	
}
