using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	[HideInInspector]public TrailHandler trailHandler;
	[HideInInspector]public HidingHandler hidingHandler;
	[HideInInspector]public CollectingHandler collectingHandler;
	[HideInInspector]public SoundSource soundSource;
	public LayerMask groundLayerMask;
	public LayerMask hidingSpotLayerMask;
	public float moveSpeed;
	public float dashSpeed;
	public float dashLength;
	public float dashCooldownTime;
	public HidingSpot hide;


	Vector3 velocity, horizontalVelocity;
	Vector3 newVelModifier, prevVelModifier, velHorizontalModifier;
	float dashStartTime;
	bool dashCooldown = false;
	bool isDashing;
	bool isCrawling;
	bool isGrounded;
	bool isWalking;
	bool overridingTransform;
	Vector3 defaultForward, defaultRight;
	Vector3 oldPos;
	Collider playerCollider;
	public bool IsDashing
	{
		get {return isDashing;}
	}
	public bool IsGrounded
	{
		get {return isGrounded;}
	}
	public Vector3 CurrentVelocity
	{
		get {return velocity;}
	}
	public bool IsWalking
	{
		get { return isWalking; }
	}
	public bool IsCrawling
	{
		get { return isCrawling; }
	}
	public bool IsMoving
	{
		get {
			if (velocity.magnitude > 0.05f)
				return true;
			else
				return false;
		}
	}
	public bool OverridingTransform
	{
		get { return overridingTransform; }
		set { overridingTransform = value; }
	}
	// Use this for initialization
	void Start () {
		trailHandler = GetComponent<TrailHandler>();
		hidingHandler = GetComponent<HidingHandler>();
		collectingHandler = GetComponent<CollectingHandler>();
		

		defaultForward = Vector3.Cross(Camera.main.transform.right, Vector3.up).normalized;
 		defaultRight = Camera.main.transform.right;
		playerCollider = GetComponent<Collider>();
		soundSource = GetComponent<SoundSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		oldPos = transform.position;

		MovementInputs();
		Gravity();
		if (!overridingTransform)
			ApplyTransform();

		
		

		if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
			dashCooldown = false;
	}

	void MovementInputs()
	{
		bool x = false,z=false;

		if (!hidingHandler.IsHiding)
		{

			newVelModifier.x = 0;
			newVelModifier.z = 0;
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				z =true;
				newVelModifier.z += 1;
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				z =true;
				newVelModifier.z += -1;			
			}
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				x =true;
				newVelModifier.x += -1;			
				
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				x =true;
				newVelModifier.x += 1;			
			}

			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				if (!isDashing && isGrounded)
					isCrawling = !isCrawling;
			}
			else if (!isGrounded)
				isCrawling = false;


			velHorizontalModifier = newVelModifier;
			velHorizontalModifier.y = 0;
			velHorizontalModifier.Normalize();

			if (!x) //Slow down
				newVelModifier.x = prevVelModifier.x /2f;
			else //apply movement speed
				newVelModifier.x = velHorizontalModifier.x*moveSpeed * (isCrawling ? 0.25f : 1f);
			
			if (!z) //slow down
				newVelModifier.z = prevVelModifier.z / 2f;
			else //apply movement speed
				newVelModifier.z = velHorizontalModifier.z*moveSpeed * (isCrawling ? 0.25f : 1f);
		}

		//
		// OVERRIDES
		//

		if (!isCrawling && (Input.GetKeyDown(KeyCode.Space) || isDashing))
		{
			if (!hidingHandler.IsHiding && !isDashing && hidingHandler.AllowHiding)
			{
				if (hide)
				{
					hidingHandler.StartHiding();
					return;
					
				}

			}
			else if (hidingHandler.IsHiding)
			{
				Debug.Log("End Hiding!");
				hidingHandler.EndHiding();
				return;
			}
			else if (collectingHandler.AllowShake)
			{
				collectingHandler.ShakeTree();
				return;
			}
			
			if (!dashCooldown || isDashing)
			{
				newVelModifier.x = velHorizontalModifier.x * dashSpeed;
				newVelModifier.z = velHorizontalModifier.z * dashSpeed;
				
				if (!dashCooldown && !isDashing)
				{
					dashStartTime = Time.time;
					dashCooldown = true;
					isDashing = true;
					newVelModifier.y = dashSpeed;
				}
				
				if (dashStartTime + dashLength < Time.time)
				{
					isDashing = false;
				}
			}
			
			
		}

		if (!x && !z)
			isWalking = false;
		else
			isWalking = true;
		prevVelModifier = newVelModifier;
	}

	void Gravity()
	{
		RaycastHit hit;
		float distOffset = Mathf.Abs(oldPos.y-transform.position.y) + 0.01f;
		bool highSpeed = false;
		if (velocity.y < -20f)
		{
			distOffset += Mathf.Abs(velocity.y/50f);
			highSpeed = true;
			Debug.Log("Falling at a High Speed");

		}

		isGrounded = Physics.Raycast(new Vector3(transform.position.x, oldPos.y, transform.position.z),-Vector3.up,out hit, playerCollider.bounds.extents.y + distOffset,groundLayerMask);


		if(isGrounded)
		{
			if (newVelModifier.y < 0)
				newVelModifier.y = 0;

			//Make sure not to fall through
			if (transform.position.y - playerCollider.bounds.extents.y < hit.point.y-0.01f || highSpeed)
				transform.position = new Vector3(transform.position.x, hit.point.y +playerCollider.bounds.extents.y, transform.position.z);
			
		}
		else
		{
			newVelModifier.y -= 0.981f;
		}
	}

	void ApplyTransform()
	{
		//Convert modifier directions into camera-forward
		velocity = defaultForward*newVelModifier.z;
		velocity += defaultRight*newVelModifier.x;
		//Apply gravity as raw
		velocity.y = newVelModifier.y;

		transform.position += velocity*Time.deltaTime;


		//Get horizontal velocity to calculate character rotation
		horizontalVelocity = new Vector3(velocity.x,0,velocity.z);
		transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, horizontalVelocity, Time.deltaTime*10f, 0.0f));
	}
}
