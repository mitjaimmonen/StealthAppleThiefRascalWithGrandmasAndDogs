using System.Collections;
using System.Collections.Generic;
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
    public float sentryModeDuration = 3;
    public float viewAngle;
    public float sentryModeLookAngle;
    public Vector3 startPosition;
    public bool sentry;
    public bool triggered;

    public Path _path;


    public float _waypointArriveDistance = 0.5f;

    public Direction _direction;

    private IMover _mover;
    public IMover Mover { get { return _mover; } }

    private StateMachine stateMachine { get; set; }

    private void Awake()
    {
        startPosition = transform.position;
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

        yield return new WaitForSeconds(sentryModeDuration / 16);


        sentry = false;
    }

    public IEnumerator OverridenSentry(Waypoint waypoint)
    {

        Debug.Log("Sentry mode override");

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

        yield return new WaitForSeconds(waypoint.sentryModeDuration / 16);

        sentry = false;
    }

    public void Trigger(bool isAlert, bool hghPriority, Transform target)
    {
        if (!triggered)
        {

        }
    }
}
