using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointsManager))]
public class PointsManagerEditor : Editor
{
    PointsManager _base;


    public override void OnInspectorGUI()
    {
        _base = (PointsManager)target;

        if (GUILayout.Button("Create Points"))
        {
            _base.points = new List<Vector2>();
            for (int i = 0; i < _base.numberOfPoints; i++)
            {
                _base.points.Add(new Vector2(Random.Range(-_base.area, _base.area), Random.Range(-_base.area, _base.area)));
            }
            _base.OnButtonClick();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("UpdateHull"))
        {
            _base.OnUpdateHull();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Triangulation"))
        {
            _base.OnTriangulate();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("CheckDelaunayValidity"))
        {
            _base.CheckDelaunayValidity();
            SceneView.RepaintAll();
        }

        base.OnInspectorGUI();




    }
}
