using System.Collections;
using UnityEngine;
using WaypointSystem;
using AIStateStuff;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    public bool switchState = false;
    public float gameTimer;
    public int seconds = 0;
    public float _moveSpeed = 5;
    public float _turnSpeed = 8;
    public float _moveSpeedChaseModifier;
    public float _turnSpeedChaseModifier;

    public float sentryModeDuration = 3;
    public float viewAngle;
    public float sentryModeLookAngle;
    public float giveUpChaseDistance = 17;
    public Transform target;
    private bool followingTrail;
    public TrailType detectable;
    public NavMeshAgent navMeshAgent;
    public WeaponParticleLauncher weapon;

    public GameObject questionMark;
    public GameObject exclamationMark;

    public Vector3 startPosition;
    public bool sentry;
    public bool triggered;
    public bool heardOutOfPatrol;

    public Path _path;

    public Animator animator;

    public bool moving;
    public float orientation;

    public float _waypointArriveDistance = 0.5f;
    public float attackDistance = 1;
    public float distanceToChase = 3;

    public Direction _direction;
    public FieldOfView fieldOfView;

    private IMover _mover;
    public IMover Mover { get { return _mover; } }

    public delegate void HeardPlayer(Transform location);
    public event HeardPlayer onPlayerHeard;


    public StateMachine stateMachine { get; protected set; }

    private void Awake()
    {
        startPosition = transform.position;
        fieldOfView = GetComponent<FieldOfView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Init()
    {
        _mover = gameObject.GetOrAddComponent<TransformMover>();
        _mover.Init(_moveSpeed, _turnSpeed);
    }

    private void Start()
    {
        stateMachine = new StateMachine(this);
        gameTimer = Time.time;
        fieldOfView.detectEvent += Trigger;
        GameMaster.Instance.alertEvent += JumpToChase;

        Init();
        stateMachine.InitStates();
    }

    private void Update()
    {
        stateMachine.ExecuteStateUpdate();

        if (sentry)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.updateRotation = false;
            moving = false;
        }        
        else
        {
            navMeshAgent.isStopped = false;
            orientation = 0;
            moving = true;
        }

        if (stateMachine.CurrentState._state != AIStateType.Cautious)
        {
            questionMark.SetActive(false);
        }

        if (stateMachine.CurrentState._state != AIStateType.Chase)
        {
            exclamationMark.SetActive(false);
        }

        UpdateAnimator();

    }

    public void UpdateAnimator()
    {
        animator.SetBool("Walking", moving);
        animator.SetFloat("Orientation", orientation);        
        animator.SetBool("Alert",(stateMachine.CurrentState._state==AIStateType.Chase));
        

    }

    public IEnumerator Sentry()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.isStopped = true;

        sentry = true;

        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion qMinus = Quaternion.AngleAxis(-sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion qPlus = Quaternion.AngleAxis(sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion lastRotation = transform.rotation;


        while (elapsedTime <= sentryModeDuration)
        {
            if (!triggered)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < sentryModeDuration / 3)
                {
                    //rotate to one side
                    transform.rotation = Quaternion.Slerp(startRotation, qMinus, (elapsedTime / ((sentryModeDuration / 3))));
                    lastRotation = transform.rotation;
                    orientation = -1 * (elapsedTime / ((sentryModeDuration / 3)));
                }
                else if (elapsedTime > sentryModeDuration / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - sentryModeDuration / 2) / (sentryModeDuration / 2)));
                    orientation = 1 * ((elapsedTime - sentryModeDuration / 2) / (sentryModeDuration / 2));
                }

                yield return new WaitForEndOfFrame();
            }
            else
            {
                sentry = false;
                triggered = false;
                yield break;
            }
        }

        if (!triggered)
            yield return new WaitForSeconds(sentryModeDuration / 16);


        sentry = false;
        navMeshAgent.updateRotation = true;
    }

    public IEnumerator SentryForFollowing(float lookAngle, float waitTime)
    {
        Debug.Log("looking for racoon on trash");
        navMeshAgent.updateRotation = false;
        navMeshAgent.isStopped = true;
        sentry = true;

        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion qMinus = Quaternion.AngleAxis(-lookAngle / 2, transform.up) * transform.rotation;
        Quaternion qPlus = Quaternion.AngleAxis(lookAngle / 2, transform.up) * transform.rotation;
        Quaternion lastRotation = transform.rotation;


        while (elapsedTime <= waitTime)
        {
            if (!triggered)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < waitTime / 3)
                {
                    //rotate to one side
                    transform.rotation = Quaternion.Slerp(startRotation, qMinus, (elapsedTime / ((waitTime / 3))));
                    lastRotation = transform.rotation;
                     orientation = -1 * (elapsedTime / ((sentryModeDuration / 3)));
                }
                else if (elapsedTime > waitTime / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - waitTime / 2) / (waitTime / 2)));
                      orientation = 1 * ((elapsedTime - sentryModeDuration / 2) / (sentryModeDuration / 2));
                }

                yield return new WaitForEndOfFrame();
            }
            else
            {
                sentry = false;
                triggered = false;
                yield break;
            }
        }

        if (!triggered)
        {
            yield return new WaitForSeconds(waitTime / 16);
        }


        sentry = false;
        navMeshAgent.updateRotation = true;
    }

    public IEnumerator OverridenSentry(Waypoint waypoint)
    {
        if (waypoint.sentryModeDuration < 0.1f)
        {
            yield break;
        }

        navMeshAgent.isStopped = true;
        navMeshAgent.updateRotation = false;
        sentry = true;

        Quaternion startRotation = transform.rotation;

        while (transform.forward != waypoint.transform.forward)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, waypoint.transform.rotation, 3 * Time.deltaTime);            
            yield return null;
        }

        float elapsedTime = 0;
        startRotation = transform.rotation;
        Quaternion qMinus = Quaternion.AngleAxis(-waypoint.sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion qPlus = Quaternion.AngleAxis(waypoint.sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion lastRotation = transform.rotation;

        while (!triggered && waypoint.stationary)
        {
            while (elapsedTime <= waypoint.sentryModeDuration && !triggered)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < waypoint.sentryModeDuration / 3)
                {
                    //rotate to one side
                    transform.rotation = Quaternion.Slerp(startRotation, qMinus, (elapsedTime / ((waypoint.sentryModeDuration / 3))));
                     orientation = -1 * (elapsedTime / ((sentryModeDuration / 3)));
                    lastRotation = transform.rotation;
                }
                else if (elapsedTime > waypoint.sentryModeDuration / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - waypoint.sentryModeDuration / 2) / (waypoint.sentryModeDuration / 2)));
                      orientation = 1 * ((elapsedTime - sentryModeDuration / 2) / (sentryModeDuration / 2));
                }

                if(triggered)
                    yield break;

                yield return null;

            }
            elapsedTime = 0;
            startRotation = transform.rotation;
           
            if(triggered)
                yield break;
                    

            yield return null;
        }

        if (!triggered)
            yield return new WaitForSeconds(waypoint.sentryModeDuration / 16);

        else
        {
            sentry = false;
            triggered = false;
            yield break;
        }

        sentry = false;
        navMeshAgent.updateRotation = true;
    }

    public IEnumerator DetectDelay(float waitTime, AIStateType aIState)
    {
        sentry = true;
        float counter = 0;

        if (aIState == AIStateType.Cautious)
        {
            Vector3 originalScale = questionMark.transform.localScale;
            questionMark.SetActive(true);

            while (counter < waitTime)
            {
                questionMark.transform.localScale = Vector3.Lerp(questionMark.transform.localScale, originalScale * 1.25f, counter / waitTime);
                counter += Time.deltaTime;
                yield return null;
            }

            counter = 0;
            sentry = false;

            while (counter < waitTime)
            {
                questionMark.transform.localScale = Vector3.Lerp(questionMark.transform.localScale, originalScale, counter / waitTime);
                counter += Time.deltaTime;
                yield return null;
            }

        }

        else
        {
            Vector3 originalScale = exclamationMark.transform.localScale;
            exclamationMark.SetActive(true);

            while (counter <= waitTime)
            {
                exclamationMark.transform.localScale = Vector3.Lerp(exclamationMark.transform.localScale, originalScale * 1.25f, counter / waitTime);
                counter += Time.deltaTime;
                yield return null;
            }

            counter = 0;
            sentry = false;

            while (counter < waitTime * 2)
            {
                exclamationMark.transform.localScale = Vector3.Lerp(exclamationMark.transform.localScale, originalScale / 2, counter / (2 * waitTime));
                counter += Time.deltaTime;
                yield return null;
            }

            exclamationMark.SetActive(false);
            exclamationMark.transform.localScale = originalScale;
        }
    }

    public void Trigger(Transform target, ViewType vType)
    {
        if (sentry)
            triggered = true;
    }

    public IEnumerator Attack()
    {       
        sentry = true;
        animator.SetTrigger("Attack");
        //yield return new WaitForSeconds(0.1f);
        if (weapon != null)
        {
            GameMaster.Instance.SoundMaster.SoundShootBlunderbuss(transform.position);
            weapon.Shoot();
            yield return new WaitForSeconds(1f);

        }
        else
        {         
            if (Physics.CheckSphere(transform.position + transform.forward, 0.3f, fieldOfView.playerMask))
            {
                Debug.Log("fog bite!!");
                GameMaster.Instance.player.GetHit(false);
            }
            yield return new WaitForSeconds(1f);

        }
        sentry = false;


    }

    public void Hear(Transform playerLastPos, bool isPLayer)
    {
        //caution mode
        if (isPLayer)
        {
            if (onPlayerHeard != null)
            {
                onPlayerHeard(playerLastPos);
                triggered = true;

            }
        }
    }

    private void JumpToChase(Transform _target)
    {

        Debug.Log("called jump to chase, current state is: " + stateMachine.CurrentState._state);
        if (stateMachine.CurrentState._state != AIStateType.Chase)
            stateMachine.PerformTransition(AIStateType.Chase);
    }


    //public Transform SortPriorityTarget(AIStateType currentState, Transform _target)
    //{
    //    if (_target == target || target.tag == "Player")
    //    {
    //        return target;
    //    }

    //    if (!followingTrail)
    //    {
    //        target = _target;
    //    }

    //    if (_target.tag == "Player")
    //    {
    //        target = _target;
    //    }

    //    return target;

    //}
}
