using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamControll : MonoBehaviour
{
    public float ZoomSpeed = 0.15f;
    public float pointsSizeRatio = 20;
    public float moveSpeed = 0.25f;
    private Camera cam;

    private Vector2 prevMousePos = Vector2.zero;

	// Use this for initialization
	void Awake ()
    {
        cam = Camera.main;
        for (int i = 0; i < PointsManager.get().pointsTr.Count; i++)
        {
            PointsManager.get().pointsTr[i].localScale = Vector3.one * cam.orthographicSize / pointsSizeRatio;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            cam.orthographicSize *= 1 - ZoomSpeed;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            cam.orthographicSize *= 1 + ZoomSpeed;
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1f, 10000f);

        if(Input.mouseScrollDelta != Vector2.zero)
        {
            for (int i = 0; i < PointsManager.get().pointsTr.Count; i++)
            {
                PointsManager.get().pointsTr[i].localScale = Vector3.one * cam.orthographicSize / pointsSizeRatio;
            }
        }


        if (Input.GetMouseButtonDown(2)) prevMousePos = Input.mousePosition;

        if(Input.GetMouseButton(2))
        {
            Vector2 mouseDelta = new Vector2(Input.mousePosition.x - prevMousePos.x, Input.mousePosition.y - prevMousePos.y) * -moveSpeed;

            cam.transform.Translate(mouseDelta * Time.deltaTime * cam.orthographicSize);

            prevMousePos = Input.mousePosition;
        }


	}
}
