using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    [RuntimeInitializeOnLoadMethod]
    public static void Instanciate()
    {
        GameObject instance = new GameObject("ChangeScene manager");
        DontDestroyOnLoad(instance);
        ChangeScene sc = instance.AddComponent<ChangeScene>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
        if(Input.GetKeyDown(KeyCode.F1))
        {
            GoToTriangulation();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GoToEnvelope();
        }

    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.alignment = TextAnchor.UpperRight;
        float size = 100;
        Rect rightRect = new Rect(Screen.width - size, 0, size, 200);


        GUILayout.BeginArea(rightRect);
        if(GUILayout.Button("Triangulation", style))
        {
            GoToTriangulation();
        }
        else if(GUILayout.Button("Envelope 3D", style))
        {
            GoToEnvelope();
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("QUIT", style))
        {
            Application.Quit();
        }


        GUILayout.EndArea();
    }

    public void GoToTriangulation()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToEnvelope()
    {
        SceneManager.LoadScene(1);
    }

}
