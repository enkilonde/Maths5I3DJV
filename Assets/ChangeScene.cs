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
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene(1);
        }

    }
}
