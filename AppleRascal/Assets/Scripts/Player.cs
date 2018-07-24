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


	float dashStartTime;
	bool dashCooldown = false;
	bool isDashing;
	// Use this for initialization
	void Start () {
		if (!trailHandler)
			trailHandler = GetComponent<TrailHandler>();
		
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		MovementInputs();

		if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
			dashCooldown = false;
	}

	void MovementInputs()
	{
		var localVelocity = transform.InverseTransformDirection(rb.velocity);
		Vector3 newVelocity = Vector3.zero;
		bool z = false,x = false;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			z =true;
			newVelocity.z += moveSpeed;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			z =true;
			newVelocity.z += -moveSpeed;			
		}

		
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			x =true;
			newVelocity.x += -moveSpeed;			
			
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			x =true;
			newVelocity.x += moveSpeed;			
		}

		if ((!dashCooldown && Input.GetKey(KeyCode.Space)) || isDashing)
		{
			if (!dashCooldown && !isDashing)
			{
				Debug.Log("DASH START!");
				dashStartTime = Time.time;
				dashCooldown = true;
				isDashing = true;
				newVelocity.y += dashSpeed/2;
			}
			if (dashStartTime + dashLength < Time.time)
			{
				isDashing = false;
			}
			
			Debug.Log("dashing..");
			newVelocity += newVelocity.normalized * dashSpeed;
		}


		if (!z)
			localVelocity.z /= 2f;
		else
			localVelocity.z = newVelocity.z;
		if (!x)
			localVelocity.x /= 2f;
		else
			localVelocity.x = newVelocity.x;

		rb.velocity = transform.TransformDirection(localVelocity);
	}
}
