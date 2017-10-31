using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtils
{

    public static Vector3 cursorWorldPosOnNCP
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.nearClipPlane));
        }
    }

    public static Vector3 cameraToCursor
    {
        get
        {
            return cursorWorldPosOnNCP - Camera.main.transform.position;
        }
    }

    public static Vector3 CursorWorldPosDepth(float depth)
    {
        return Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            depth));
    }

    public static Vector3 cursorOnTransform(Transform transform)
    {

        Vector3 camToTrans = transform.position - Camera.main.transform.position;
        return Camera.main.transform.position +
            cameraToCursor *
            (Vector3.Dot(Camera.main.transform.forward, camToTrans) / Vector3.Dot(Camera.main.transform.forward, cameraToCursor));
        
    }
}
