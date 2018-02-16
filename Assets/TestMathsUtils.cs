using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


using EnkiBye.Maths;
using EnkiBye.Maths.Shapes;


public class TestMathsUtils : MonoBehaviour {

    public Transform[] objects;
    public Transform[] objects2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		


	}

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        //if (objects.Length < 3) return;

        //gizmosWithoutTriangles();

        //gizmosWithTriangles();

        //polygonGizmos();

        crossGizmos();
    }

    
    public void gizmosWithTriangles()
    {

        Triangle T = new Triangle(objects[0].position, objects[1].position, objects[2].position);


        Gizmos.DrawLine(T.a, T.b);
        Gizmos.DrawLine(T.b, T.c);
        Gizmos.DrawLine(T.c, T.a);


        Handles.BeginGUI();
        GUIStyle style = GUI.skin.label;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 20;

        style.normal.textColor = Color.red;
        Handles.Label(T.a, ""+(int)T.angleAtSegment(0), style);

        style.normal.textColor = Color.green;
        Handles.Label(T.b, "" + (int)T.angleAtSegment(1), style);

        style.normal.textColor = Color.blue;
        Handles.Label(T.c, "" + (int)T.angleAtSegment(2), style);

        Handles.EndGUI();


        Gizmos.color = Color.red;

        for (int i = 0; i < T.segments.Length; i++)
        {
            Gizmos.DrawLine(T.segments[i].middle, T.segments[i].middle + T.segments[i].normal);
        }
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(T.hortocenter, Vector2.Distance(T.hortocenter, T.a));

        int sizeArray = 20;

        for (int i = -sizeArray; i < sizeArray; i++)
        {
            for (int j = -sizeArray; j < sizeArray; j++)
            {
                Vector2 pos = new Vector2(i, j);
                Gizmos.color = pos.isInTriangle(T)  ?  Color.green  :  (pos.isInCircumscribedCircle(T)  ?  Color.blue  :  Color.black);
                Gizmos.DrawSphere(pos, 0.25f);
                if (T.ContainPoint(pos))
                {
                    Gizmos.DrawWireSphere(pos, 0.35f);
                }
            }
        }









    }

    public void polygonGizmos()
    {
        List<Vector2> vec = new List<Vector2>();
        for (int i = 0; i < objects2.Length; i++)
        {
            vec.Add(objects2[i].position);
        }
        Polygon poly = new Polygon(vec.ToArray());

        poly.Draw();

        int sizeArray = 20;

        for (int i = -sizeArray; i < sizeArray; i++)
        {
            for (int j = -sizeArray; j < sizeArray; j++)
            {
                Vector2 pos = new Vector2(i, j);
                if(poly.isPointOnEdge(pos))
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                if (poly.ContainPoint(pos) && !poly.isPointOnEdge(pos))
                {
                    Gizmos.DrawWireSphere(pos, 0.25f);
                }
                else
                {
                    Gizmos.DrawSphere(pos, 0.25f);
                }
            }
        }

    }

    public void crossGizmos()
    {
        if (objects2.Length < 3) return;

        Vector3 p1 = objects2[0].transform.position;
        Vector3 p2 = objects2[1].transform.position;
        Vector3 p3 = objects2[2].transform.position;

        Gizmos.DrawSphere(p1, 0.1f);
        Gizmos.DrawSphere(p2, 0.1f);
        Gizmos.DrawSphere(p3, 0.1f);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p2, p2 + Vector3.Cross(p2 - p1, p3 - p2));

    }


#endif

}
