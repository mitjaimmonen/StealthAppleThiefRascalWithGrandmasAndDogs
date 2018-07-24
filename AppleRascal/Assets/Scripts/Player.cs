using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	[HideInInspector]public TrailHandler trailHandler;
	public float moveSpeed;
	public float dashSpeed;
	public float dashLength;
	public float dashCooldownTime;
	Rigidbody rb;


	Vector3 direction;
	Vector3 velModifier;
	float dashStartTime;
	bool dashCooldown = false;
	bool isDashing;
	Vector3 defaultForward, defaultRight;

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
	}
	
	// Update is called once per frame
	void Update () {
		
		MovementInputs();


		transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, velModifier, Time.deltaTime*10f, 0.0f));
		
		

		if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
			dashCooldown = false;
	}

	void MovementInputs()
	{
		var localVelocity = rb.velocity;
		velModifier = Vector3.zero;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			velModifier = defaultForward;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			velModifier += -defaultForward;			
		}

		
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			velModifier += -defaultRight;			
			
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			velModifier += defaultRight;			
		}

		if ((!dashCooldown && Input.GetKey(KeyCode.Space)) || isDashing)
		{
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
			
			velModifier += velModifier.normalized * dashSpeed;
		}


		velModifier.Normalize();
		localVelocity.z = velModifier.z*moveSpeed;
		localVelocity.x = velModifier.x*moveSpeed;
		
		localVelocity.y += velModifier.y;

		rb.velocity = localVelocity;
	}
}
