using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{


    [HideInInspector] public TrailHandler trailHandler;
    [HideInInspector] public HidingHandler hidingHandler;
    [HideInInspector] public CollectingHandler collectingHandler;
    [HideInInspector] public PlayerSoundHandler playerSoundHandler;
    public NavMeshAgent navMeshAgent;
    public ParticleSystem leapParticles;
    public LayerMask groundLayerMask;
    public LayerMask hidingSpotLayerMask;
    public GameObject playerVisuals;
    public float damageRecoverTime = 5f;
    public float moveSpeed;
    public float dashSpeed;
    public float dashLength;
    public float dashCooldownTime;
    public HidingSpot hide;
    public bool finishAutomatically;


    Vector3 velocity, horizontalVelocity;
    Vector3 newVelModifier, prevVelModifier, velHorizontalModifier;
    float dashStartTime;
    bool dashCooldown = false;

	bool isDamaged;
	bool isInvisible;
	bool isDashing;
	bool isCrawling;
	bool isGrounded;
	bool isWalking;
	bool isInWater;
	bool overridingTransform;
	Vector3 defaultForward, defaultRight;
	Vector3 oldPos;
	BoxCollider playerCollider;

    private Animator animator;

    bool allowFinish;


	float damagedTime = 0;
	float waterTime = 0;
    public bool hasMoved;
    public bool hasCrawled;
    public bool hasJumped;


	public bool IsInWater
	{
		get { return isInWater; }
		set {
			waterTime = Time.time;
			isInWater = value; }
	}


    #region Getters and Setters
    public bool IsInvisible
    {
        get { return isInvisible; }
        set { isInvisible = value; }
    }
    public bool IsDashing
    {
        get { return isDashing; }
    }
    public bool IsGrounded
    {
        get { return isGrounded; }
    }
    public Vector3 CurrentVelocity
    {
        get { return velocity; }
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
        get
        {
            if (Mathf.Abs(newVelModifier.x) > 0.1f || Mathf.Abs(newVelModifier.z) > 0.1f)
                return true;
            else
                return false;
        }
    }
    public float DashOnCooldown
    {
        get
        {
            if (!dashCooldown)
                return 0;

            float cooldownLeft = (dashStartTime + dashLength + dashCooldownTime) - Time.time;
            return Mathf.Clamp(cooldownLeft /= dashCooldownTime, 0, 1f);


        }
    }

    public bool AllowFinish
    {
        get { return allowFinish; }
        set
        {
            if (finishAutomatically && value)
                EndGame(true);
            if (value != allowFinish)
                GameMaster.Instance.gameCanvas.hudHandler.SetActionText(value, "Finish level");

            allowFinish = value;
        }
    }
    public bool OverridingTransform
    {
        get { return overridingTransform; }
        set { overridingTransform = value; }
    }

    #endregion



    public void GetHit()
    {
        if (!isDamaged)
        {
            isDamaged = true;
            damagedTime = Time.time;
        }
        else
        {
            StartCoroutine(Die());
        }

    }

    IEnumerator Die()
    {
        //Play death animation.
        OverridingTransform = true;
        yield return new WaitForSeconds(1f);
        GameMaster.Instance.EndGame(false);
        yield break;
    }



    // Use this for initialization
    void Start()
    {
        trailHandler = GetComponent<TrailHandler>();
        hidingHandler = GetComponent<HidingHandler>();
        collectingHandler = GetComponent<CollectingHandler>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();


        defaultForward = Vector3.Cross(Camera.main.transform.right, Vector3.up).normalized;
        defaultRight = Camera.main.transform.right;
        playerCollider = GetComponent<BoxCollider>();
        playerSoundHandler = GetComponent<PlayerSoundHandler>();
        animator = GetComponentInChildren<Animator>();

        GameObject start = GameObject.FindGameObjectWithTag("Start");
        if (start)
            transform.parent.transform.position = start.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        oldPos = transform.position;

        MovementInputs();
        Gravity();
        if (!overridingTransform)
            ApplyTransform();

		if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
			dashCooldown = false;

		if (IsInWater && waterTime + 0.1f < Time.time)
			IsInWater = false;

        if (hidingHandler.IsHiding)
            isInvisible = true;
        else
            isInvisible = false;


        if (IsCrawling)
        {
            var size = playerCollider.size;
            size.y = 1f;
            playerCollider.size = size;
            var center = playerCollider.center;
            center.y = 0;
            playerCollider.center = center;
            navMeshAgent.height = 1f;
        }
        else
        {
            var size = playerCollider.size;
            size.y = 2f;
            playerCollider.size = size;
            var center = playerCollider.center;
            center.y = 0.5f;
            playerCollider.center = center;
            navMeshAgent.height = 2f;
        }

        if (isDamaged && damagedTime + damageRecoverTime < Time.time)
            isDamaged = false;

        if (dashCooldown && dashStartTime + dashLength + dashCooldownTime < Time.time)
            dashCooldown = false;

        UpdateAnimator();
    }

    void EndGame(bool win)
    {
        GameMaster.Instance.EndGame(win);
    }
    void MovementInputs()
    {
        bool x = false, z = false;

        if (!hidingHandler.IsHiding)
        {

            newVelModifier.x = 0;
            newVelModifier.z = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                z = true;
                newVelModifier.z += 1;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                z = true;
                newVelModifier.z += -1;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                x = true;
                newVelModifier.x += -1;

            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                x = true;
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
                newVelModifier.x = prevVelModifier.x / 2f;
            else //apply movement speed
                newVelModifier.x = velHorizontalModifier.x * moveSpeed * (isCrawling ? 0.25f : 1f);

            if (!z) //slow down
                newVelModifier.z = prevVelModifier.z / 2f;
            else //apply movement speed
                newVelModifier.z = velHorizontalModifier.z * moveSpeed * (isCrawling ? 0.25f : 1f);
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
                        // hasJumped = true;
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
                    // hasJumped = true;
                    collectingHandler.ShakeTree();
                }
                else if (AllowFinish)
                {
                    EndGame(true);
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
                    newVelModifier.y = dashSpeed / 5f;
                    hasJumped = true;
                    animator.SetTrigger("Jump");
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
        if ((!x && !z) || hidingHandler.IsHiding)
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
        velocity = defaultForward * newVelModifier.z;
        velocity += defaultRight * newVelModifier.x;
        //Apply gravity as raw
        velocity.y = 0;

        transform.parent.transform.position += velocity * Time.deltaTime;

        //Local positions
        velocity.y = newVelModifier.y;
        transform.localPosition += new Vector3(0, velocity.y * Time.deltaTime + (-9.81f * Time.deltaTime * Time.deltaTime / 2f), 0);
        if (transform.localPosition.y < 0)
        {
            transform.localPosition = Vector3.zero;
            newVelModifier.y = 0;
            isGrounded = true;
        }
        else if (transform.localPosition.y > 0.1f)
            isGrounded = false;


        //Get horizontal velocity to calculate character rotation
        horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        transform.parent.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, horizontalVelocity, Time.deltaTime * 10f, 0.0f));
    }

    void UpdateAnimator()
    {
        animator.speed = 1;

        animator.SetBool("Crawling", isCrawling);
        animator.SetBool("Die", isDamaged);
        if (isCrawling)
        {
            animator.SetBool("Running", false);
            if (!isWalking && animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl"))
                animator.speed = 0;
        }
        else
        {
            animator.SetBool("Running", isWalking);
        }
    }
}
