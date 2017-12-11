using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple3DCamControll : MonoBehaviour
{
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;

    Quaternion originalRotation;

    public float moveSpeed = 50;

    bool focusCenter = false;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    void Update()
    {

        Rotate();
        Move();

        if (focusCenter) transform.LookAt(Vector3.zero);

        if (Input.GetMouseButtonUp(2)) focusCenter = !focusCenter;


    }



    public void Rotate()
    {
        //if (!Input.GetMouseButton(1)) return;

        Transform target = null;

        if (Input.GetMouseButton(1)) target = Camera.main.transform;
        if (Input.GetMouseButton(0))
        {
            target = FindObjectOfType<Light>().transform;
        }

        if (target == null) return; 

        rotAverageY = 0f;
        rotAverageX = 0f;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;

        rotArrayY.Add(rotationY);
        rotArrayX.Add(rotationX);

        if (rotArrayY.Count >= frameCounter)
        {
            rotArrayY.RemoveAt(0);
        }
        if (rotArrayX.Count >= frameCounter)
        {
            rotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < rotArrayY.Count; j++)
        {
            rotAverageY += rotArrayY[j];
        }
        for (int i = 0; i < rotArrayX.Count; i++)
        {
            rotAverageX += rotArrayX[i];
        }

        rotAverageY /= rotArrayY.Count;
        rotAverageX /= rotArrayX.Count;

        rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
        rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

        Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

        target.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    public void Move()
    {
        transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position += transform.up * Input.GetAxis("UpDown") * Time.deltaTime * moveSpeed;
        transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        //transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
    }


    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}
