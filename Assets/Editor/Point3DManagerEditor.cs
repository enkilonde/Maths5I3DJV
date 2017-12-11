using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(points3DManager))]
public class Point3DManagerEditor : Editor
{
    points3DManager _base;

    public override void OnInspectorGUI()
    {

        _base = (points3DManager)target;

        if(GUILayout.Button("randomise Point"))
        {
            _base.CreateNewPoint();
        }

        if (GUILayout.Button("Add point"))
        {
            _base.addNewPoint(_base.newPoint);
        }


        base.OnInspectorGUI();



    }


}
