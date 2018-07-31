using System.Collections;
using UnityEngine;
using WaypointSystem;
using AIStateStuff;

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



    public Vector3 startPosition;
    public bool sentry;
    public bool triggered;

    public Path _path;


    public float _waypointArriveDistance = 0.5f;
    public float attackDistance = 1;
    public float distanceToChase = 3;

    public Direction _direction;
    public FieldOfView fieldOfView;

    private IMover _mover;
    public IMover Mover { get { return _mover; } }

    public StateMachine stateMachine { get; protected set; }

    private void Awake()
    {
        startPosition = transform.position;
        fieldOfView = GetComponent<FieldOfView>();
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

        Init();
        stateMachine.InitStates();
    }

    private void Update()
    {
        stateMachine.ExecuteStateUpdate();
    }

    public IEnumerator Sentry()
    {
        Debug.Log("Sentry mode");
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
                }
                else if (elapsedTime > sentryModeDuration / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - sentryModeDuration / 2) / (sentryModeDuration / 2)));
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
    }

    public IEnumerator SentryForFollowing(float lookAngle, float waitTime)
    {
        sentry = true;
        Debug.Log("starting sentry for caution");
       

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
                }
                else if (elapsedTime > waitTime / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - waitTime / 2) / (waitTime / 2)));
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
    }

    public IEnumerator OverridenSentry(Waypoint waypoint)
    {
        if (waypoint.sentryModeDuration < 0.1f)
        {
            yield break;
        }

        sentry = true;

        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion qMinus = Quaternion.AngleAxis(-waypoint.sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion qPlus = Quaternion.AngleAxis(waypoint.sentryModeLookAngle / 2, transform.up) * transform.rotation;
        Quaternion lastRotation = transform.rotation;


        while (elapsedTime <= waypoint.sentryModeDuration)
        {
            if (!triggered)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < waypoint.sentryModeDuration / 3)
                {
                    //rotate to one side
                    transform.rotation = Quaternion.Slerp(startRotation, qMinus, (elapsedTime / ((waypoint.sentryModeDuration / 3))));
                    lastRotation = transform.rotation;
                }
                else if (elapsedTime > waypoint.sentryModeDuration / 2)
                {
                    //rotate to other side
                    transform.rotation = Quaternion.Slerp(lastRotation, qPlus, ((elapsedTime - waypoint.sentryModeDuration / 2) / (waypoint.sentryModeDuration / 2)));
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
            yield return new WaitForSeconds(waypoint.sentryModeDuration / 16);

        sentry = false;
    }

    public void Trigger(Transform target)
    {
        if (sentry)
            triggered = true;
    }

    public IEnumerator Attack()
    {
        sentry = true;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("attacked!!");
        yield return new WaitForSeconds(1f);
        sentry = false;
        
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
