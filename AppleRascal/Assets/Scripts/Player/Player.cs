using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {


	[HideInInspector]public TrailHandler trailHandler;
	[HideInInspector]public HidingHandler hidingHandler;
	[HideInInspector]public CollectingHandler collectingHandler;
	[HideInInspector]public SoundSource soundSource;
	public ParticleSystem leapParticles;
	public LayerMask groundLayerMask;
	public LayerMask hidingSpotLayerMask;
	public float moveSpeed;
	public float dashSpeed;
	public float dashLength;
	public float dashCooldownTime;
	public HidingSpot hide;
	public bool finishAutomatically;
	
	

	NavMeshAgent navMeshAgent;


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

	bool allowFinish;

	
	public bool hasMoved;
	public bool hasCrawled;
	public bool hasJumped;



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

	public bool AllowFinish
	{
		get { return allowFinish; }
		set {
			if (finishAutomatically && value)
				Finish();
			if (value != allowFinish)
				GameMaster.Instance.hudHandler.SetActionText(value, "Finish level");

			allowFinish = value;
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
		navMeshAgent = GetComponentInParent<NavMeshAgent>();
		

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

	void Finish()
	{
		Debug.Log("FINISHED LEVEL WOOOOOO YEAA");
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
				{
					isCrawling = !isCrawling;
					hasCrawled = true;
				}
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
			if (!isDashing)
			{
				if (!hidingHandler.IsHiding && hidingHandler.AllowHiding)
				{
					if (hide)
					{
						hidingHandler.StartHiding();
					}

				}
				else if (hidingHandler.IsHiding)
				{
					Debug.Log("End Hiding!");
					hidingHandler.EndHiding();
				}
				else if (collectingHandler.AllowShake)
				{
					collectingHandler.ShakeTree();
				}
				else if (AllowFinish)
				{
					Finish();
				}
			}
			
			if ((!dashCooldown || isDashing) && (!collectingHandler.AllowShake && !hidingHandler.AllowHiding))
			{
				newVelModifier.x = velHorizontalModifier.x * dashSpeed;
				newVelModifier.z = velHorizontalModifier.z * dashSpeed;
				
				if (!dashCooldown && !isDashing)
				{
					dashStartTime = Time.time;
					dashCooldown = true;
					isDashing = true;
					newVelModifier.y = dashSpeed/5f;
					hasJumped = true;
					
					if (leapParticles)
					{
						var main = leapParticles.main;
						main.startSpeedMultiplier = velocity.magnitude;
						leapParticles.Play();
					}
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
		{
			isWalking = true;
			hasMoved = true;
		}
		prevVelModifier = newVelModifier;
	}

	void Gravity()
	{

		newVelModifier.y -= 9.81f * Time.deltaTime;
	
	}

	void ApplyTransform()
	{
		//Convert modifier directions into camera-forward
		velocity = defaultForward*newVelModifier.z;
		velocity += defaultRight*newVelModifier.x;
		//Apply gravity as raw
		velocity.y = 0;

		transform.parent.transform.position += velocity*Time.deltaTime;

		//Local positions
		velocity.y = newVelModifier.y;
		transform.localPosition += new Vector3 (0, velocity.y*Time.deltaTime + (-9.81f * Time.deltaTime * Time.deltaTime / 2f), 0);
		if (transform.localPosition.y < 0)
		{
			transform.localPosition = Vector3.zero;
			newVelModifier.y = 0;
			isGrounded = true;
		}
		else if (transform.localPosition.y > 0.1f)
			isGrounded = false;


		//Get horizontal velocity to calculate character rotation
		horizontalVelocity = new Vector3(velocity.x,0,velocity.z);
		transform.parent.transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, horizontalVelocity, Time.deltaTime*10f, 0.0f));
	}
}
