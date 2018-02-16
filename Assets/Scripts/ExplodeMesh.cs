using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeMesh : MonoBehaviour
{

    public AnimationCurve pointsPos;
    public int numberOfPoints = 100;


    float explosionStrenght = 0;

    public Transform explosionDebug;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	
        if(Input.GetMouseButtonDown(1))
        {
            explosionStrenght = 0;
        }

        if (Input.GetMouseButton(1))
        {
            explosionStrenght += Time.deltaTime;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    explosionDebug.gameObject.SetActive(true);
                    explosionDebug.transform.position = hit.point;
                    explosionDebug.localScale = Vector3.one * explosionStrenght;
                }
            }
            else
                explosionDebug.gameObject.SetActive(false);

        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform != null)
                {
                    explodeWall(explosionStrenght, hit.transform.GetComponent<MeshFilter>(), hit.point);

                }
            }
            explosionDebug.gameObject.SetActive(false);

        }

    }

    public void explodeWall(float power, MeshFilter meshFilt, Vector3 hitPos)
    {



        FractureMesh fm = new FractureMesh();

        fm.victim = meshFilt;

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 randomPos = Random.onUnitSphere * pointsPos.Evaluate(Random.Range(0f, 1f) * power);
            randomPos.z = 0;
            fm.points.Add(hitPos + randomPos);
        }

        fm.explosionForce = power;

        fm.ApplyFracture();
        fm.ExplosionForce(hitPos);


    }

}
