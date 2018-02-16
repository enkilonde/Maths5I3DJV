using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    Rigidbody _Rigidbody;

    public float _Speed = 5;



    public float _JumpHeight = 10;

    [HideInInspector] public float _JumpTime = 0.1f;

     public bool _Grounded = false;

	public bool _Jumping = false;
	private float _JumpForce;


	private float _JumpMultiplier;

	private Vector3 _InitialGravity;
	public float _MaxJumpTime = 1;

    public LayerMask _GroundLayers;

	// Use this for initialization
	void Awake () 
	{
		_Rigidbody = GetComponent<Rigidbody> ();
		_InitialGravity = Physics.gravity;
	}
	
	// Update is called once per frame
	void Update () 
	{

		
		_JumpTime -= Time.deltaTime;
        Jump();
	}

	void FixedUpdate () 
	{
		Vector3 _Direction;
		Vector3 _Forward;

        _Forward = new Vector3(transform.forward.x, 0, transform.forward.z);

        _Direction = _Forward * Input.GetAxisRaw ("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");

		_Rigidbody.velocity = new Vector3(0, _Rigidbody.velocity.y, 0) + _Direction * _Speed;
        

		
		
		
	}




    private void Jump()
    {

        if (Input.GetButtonUp("Jump"))
        {
            _Jumping = false;
        }

        if (Input.GetButton("Jump") && _Jumping && _JumpTime > 0)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(_Rigidbody.velocity.x, CalculateJumpVerticalSpeed() * Time.deltaTime * 50, _Rigidbody.velocity.z);
        }



        if (_Grounded == true)
        {
            _Jumping = false;
        }


        if (_Grounded && Input.GetButton("Jump"))
        {
            _JumpTime = _MaxJumpTime;
            GetComponent<Rigidbody>().velocity = new Vector3(_Rigidbody.velocity.x, CalculateJumpVerticalSpeed() * Time.deltaTime * 50, _Rigidbody.velocity.z);
            _Jumping = true;
        }

    }



   






    // Jump








    float CalculateJumpVerticalSpeed () {
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt(2 * _JumpHeight * (-Physics.gravity.y) * (_JumpTime/_MaxJumpTime));
}









}
