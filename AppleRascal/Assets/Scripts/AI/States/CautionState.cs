using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class CautionState : State
{

    private Transform currentTarget;
    private Transform nextTarget;
    private Transform lastTarget;
    private Vector3 playerLastSeenPos;
    private bool trailIsPlayer;
    public bool hasReachedCurrentTarget;
    bool followingTrail;
    bool followingSound;
    float lookForCounter;
    public float maxTimeWithNotrail = 2;
    bool goToChase;
    bool goToPatrol;
    Vector3 lastHeardPos;

    public CautionState(AI _owner)
            : base()
    {
        _state = AIStateType.Cautious;
        Owner = _owner;
        AddTransition(AIStateType.Patrol);
        AddTransition(AIStateType.Chase);

    }

    public float SqrChaseDistance
    {
        get { return Owner.distanceToChase * Owner.distanceToChase; }
    }


    public override void Enter()
    {
        Debug.Log("Entering Caution state");
        Owner.fieldOfView.detectEvent += OnDetection;       
        currentTarget = Owner.target;
        followingTrail = true;
        goToChase = false;
        goToPatrol = false;
        trailIsPlayer = false;
        lookForCounter = 0;


        Owner.navMeshAgent.speed += Owner._moveSpeedChaseModifier;


        if (currentTarget.gameObject.tag == "Player" && !GameMaster.Instance.PlayerIsHiding())
        {
            trailIsPlayer = true;
            followingTrail = true;
            playerLastSeenPos = currentTarget.position;           
        }

        //if (!Sentry)
        {
            Owner.StartCoroutine(Owner.DetectDelay(0.5f,_state));
        }

        if (Owner.heardOutOfPatrol)
        {
            Owner.heardOutOfPatrol = false;
            followingTrail = false;        
            HeardPlayer(currentTarget.position);
        }
    }

    public void OnDetection(Transform target,ViewType vType)
    {
        //if (currentTarget == target)
        //{
        //    return;
        //}


        if (target.tag == "Player")
        {
            if (Vector3.Distance(Owner.transform.position, target.position) <= Owner.distanceToChase)
                goToChase = true;
            else
            {
                if (trailIsPlayer)
                {
                    return;
                }
                else
                {
                    //  if (!target.GetComponent<Player>().IsInvisible)
                    {
                        currentTarget = target;
                        goToChase = true;
                    }
                    //else
                    //{
                    //    goToPatrol = true;
                    //}

                }

            }
        }

        if (followingTrail)
        {
            if (trailIsPlayer)
            {
                if (target.tag == "Trail")
                {
                    if (target.GetComponent<TrailPoint>().trailPointType == Owner.detectable)
                    {
                        currentTarget = target;

                    }
                }
            }
            else
                return;
        }

        else
        {
            if (target.tag == "Trail")
            {
                if (target.GetComponent<TrailPoint>().trailPointType == Owner.detectable)
                {
                    Debug.Log("got here");
                    currentTarget = target;
                    followingTrail = true;

                }
            }
        }
    }

    private void FollowTrail()
    {
        if (currentTarget.gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(Owner.transform.position, currentTarget.position) <= 1.1f)
            {
                followingTrail = false;
                lastTarget = currentTarget;
                currentTarget = null;
                //look for next trail piece

                if (lastTarget.tag == "Trail")
                {
                    TrailPoint trailPoint = lastTarget.gameObject.GetComponent<TrailPoint>();
                    if (trailPoint.connectedTrailPoint && trailPoint.connectedTrailPoint.enabled && trailPoint.trailPointType == Owner.detectable)
                    {
                        currentTarget = trailPoint.connectedTrailPoint.transform;
                        followingTrail = true;
                    }
                    else
                    {
                        Debug.Log("got here");
                    }
                }
            }
        }

        else
        {
            if (Vector3.Distance(Owner.transform.position, currentTarget.position) <= Owner._waypointArriveDistance +0.1f)
            {
                Debug.Log("got here");
                followingTrail = false;
            }

        }
    }

    private void FollowPlayer()
    {
        if (Vector3.Distance(Owner.transform.position, playerLastSeenPos) > Owner._waypointArriveDistance + 0.1f)
        {
            Owner.navMeshAgent.SetDestination(playerLastSeenPos);
            Owner.Mover.Turn(playerLastSeenPos);
        }
        else
        {
            trailIsPlayer = false;
            Owner.StartCoroutine(Owner.SentryForFollowing(90, 1f));
        }
    }

    public override void Execute()
    {
        if (!ChangeState())
        {
            Debug.Log(Owner.gameObject.name + " is on Caution mode, following trail is: " + followingTrail);
            if (!GameMaster.Instance.PlayerIsHiding())
            {

                if (!Sentry && trailIsPlayer && !followingSound)
                {
                    FollowPlayer();
                }

                if (!Sentry && followingTrail && !followingSound)
                {
                    if (!trailIsPlayer)
                    {
                        Owner.Mover.Turn(currentTarget.position);
                        Owner.navMeshAgent.SetDestination(currentTarget.position);
                        FollowTrail();

                        lookForCounter = 0;
                    }

                }
                if (!Sentry && !followingTrail && !followingSound)
                {
                    Owner.StartCoroutine(Owner.SentryForFollowing(45, 2));
                }
                if (Sentry && !trailIsPlayer && !followingSound)
                {
                    lookForCounter += Time.deltaTime;
                    if (lookForCounter >= 2)
                    {
                        goToPatrol = true;

                    }
                }
                if (!Sentry && followingSound)
                {
                    Owner.Mover.Turn(lastHeardPos);
                    Owner.navMeshAgent.SetDestination(lastHeardPos);
                    FollowSound();
                }
            }
            else
            {
                if (!Sentry)
                Owner.StartCoroutine(Owner.SentryForFollowing(45, 2));
                if (Sentry)
                lookForCounter += Time.deltaTime;
                if (lookForCounter >= 2)
                {
                    goToPatrol = true;

                }
            }

        }
    }

    void FollowSound()
    {
        if (Vector3.Distance(Owner.transform.position, lastHeardPos) > Owner._waypointArriveDistance + 0.1f)
        {
            Owner.navMeshAgent.SetDestination(lastHeardPos);
            Owner.Mover.Turn(lastHeardPos);
        }
        else
        {
            followingSound = false;
            Owner.StartCoroutine(Owner.SentryForFollowing(90, 1f));
        }
    }

    private void HeardPlayer(Vector3 soundLocation)
    {
        if (!followingTrail)
        {
            followingSound = true;
            lastHeardPos = soundLocation;
            Owner.navMeshAgent.SetDestination(soundLocation);
        }
    }

    public override void Exit()
    {
        Owner.navMeshAgent.speed -= Owner._moveSpeedChaseModifier;
        Owner.fieldOfView.detectEvent -= OnDetection;     


        if (goToChase)
            Owner.target = currentTarget;
        Debug.Log("eXITING Caution state");
    }

    private bool ChangeState()
    {
        if (goToChase)
        {
            Debug.Log("go to chase!!");
            return Owner.stateMachine.PerformTransition(AIStateType.Chase);
        }
        if (goToPatrol)
        {
            Debug.Log("go to patrol");
            return Owner.stateMachine.PerformTransition(AIStateType.Patrol);
        }

        return false;

    }
}


