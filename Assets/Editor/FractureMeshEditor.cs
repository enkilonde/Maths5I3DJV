using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FractureMesh))]
public class FractureMeshEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FractureMesh _target = (FractureMesh)target;

        if (GUILayout.Button("Calc Voronoi"))
        {
            _target.Voronoi();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Fracture"))
        {
            _target.Fracture();
            SceneView.RepaintAll();
        }

    }


    private void OnSceneGUI()
    {

        FractureMesh _target = (FractureMesh)target;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        //style.alignment = TextAnchor.MiddleCenter;
        
        if(_target.showIndices)
        {
            for (int i = 0; i < _target.fracturePoints.Count; i++)
            {
                if (i != _target.gizmoPolyIndex && _target.gizmoPolyIndex != -1) continue;

                //style.normal.textColor = FractureMesh.MeshColors[i];
                style.normal.textColor = Color.white;
                for (int j = 0; j < _target.fracturePoints[i].Count; j++)
                {

                    Handles.Label(_target.fracturePoints[i][j], "" + j, style);

                }
            }
        }

    }

}
