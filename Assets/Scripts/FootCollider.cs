using UnityEngine;

public class FootCollider : MonoBehaviour {

	private Move _Move;

	// Use this for initialization
	void Start () 
	{
		_Move = transform.parent.GetComponent<Move> ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerStay(Collider _Coll)
	{
        //print(_Move._GroundLayers.value);
        if (_Move._GroundLayers == (_Move._GroundLayers | 1 << _Coll.gameObject.layer))
        {
            _Move._Grounded = true;
        }
        
    }

    void FixedUpdate()
    {
       _Move._Grounded = false;
    }


}
