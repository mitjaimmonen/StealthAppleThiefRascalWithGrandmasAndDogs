using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	[HideInInspector]public TrailHandler trailHandler;
	public LayerMask groundLayerMask;
	public float moveSpeed;
	public float dashSpeed;
	public float dashLength;
	public float dashCooldownTime;
	Rigidbody rb;


	Vector3 velocity;
	Vector3 velModifier;
	float dashStartTime;
	bool dashCooldown = false;
	bool isDashing;
	Vector3 defaultForward, defaultRight;
	Collider playerCollider;

	public bool IsDashing
	{
		get {return isDashing;}
	}
	// Use this for initialization
	void Start () {
		if (!trailHandler)
			trailHandler = GetComponent<TrailHandler>();
		
		rb = GetComponent<Rigidbody>();

		defaultForward = Vector3.Cross(Camera.main.transform.right, Vector3.up).normalized;
 		defaultRight = Camera.main.transform.right;
		playerCollider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Vector3 oldPos = transform.position;

		MovementInputs();
		Gravity();

		transform.position +=velModifier*Time.deltaTime;
		velocity = transform.position - oldPos;

		// transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, velocity, Time.deltaTime*10f, 0.0f));
		
		

		if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
			dashCooldown = false;
	}

	void MovementInputs()
	{
		velModifier = Vector3.zero;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			velModifier = defaultForward*moveSpeed;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			velModifier += -defaultForward*moveSpeed;			
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			velModifier += -defaultRight*moveSpeed;			
			
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			velModifier += defaultRight*moveSpeed;			
		}

		if ((!dashCooldown && Input.GetKey(KeyCode.Space)) || isDashing)
		{
			velModifier += velModifier.normalized * dashSpeed;
			
			if (!dashCooldown && !isDashing)
			{
				dashStartTime = Time.time;
				dashCooldown = true;
				isDashing = true;
				velModifier.y += dashSpeed/20f;
			}
			
			if (dashStartTime + dashLength < Time.time)
			{
				isDashing = false;
			}
			
		}
	}

	void Gravity()
	{
		bool isHit = Physics.Raycast(transform.position + (-transform.up * playerCollider.bounds.extents.y),-transform.up, 0.1f);

		if(isHit)
		{
			Debug.Log("Grounded");
			if (velModifier.y < 0)
				velModifier.y = 0;
		}
		else
		{
			velModifier.y = velocity.y - 9.81f;
		}
	}
}
