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
    float lookForCounter;
    public float maxTimeWithNotrail = 2;
    bool goToChase;
    bool goToPatrol;

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


        if (currentTarget.gameObject.tag == "Player")
        {
            trailIsPlayer = true;
            followingTrail = true;
            playerLastSeenPos = currentTarget.position;
            if (!Sentry)
            {
                Owner.StartCoroutine(Owner.SentryForFollowing(0, 0.5f));
            }
        }
    }

    public void OnDetection(Transform target)
    {
        if (currentTarget == target)
        {
            return;
        }

        
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
                    currentTarget = target;
                    goToChase = true;
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
            if (Vector3.Distance(Owner.transform.position, currentTarget.position) <= 1.5f)
                followingTrail = false;

        }
    }

      private void FollowPlayer()
    {

        if (Vector3.Distance(Owner.transform.position, playerLastSeenPos) >1.5f)
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
            if (!Sentry && trailIsPlayer)
            {                
                FollowPlayer();
            }

            if (!Sentry && followingTrail)
            {               
                if (!trailIsPlayer)
                {                
                    Owner.Mover.Turn(currentTarget.position);
                    Owner.navMeshAgent.SetDestination(currentTarget.position);               
                    FollowTrail();

                    lookForCounter = 0;
                }

            }
            if (!Sentry && !followingTrail)
            {
                Owner.StartCoroutine(Owner.SentryForFollowing(45, 2));
            }
            if (Sentry && !trailIsPlayer)
            {
                lookForCounter += Time.deltaTime;              
                if (lookForCounter >= 2)
                {
                    goToPatrol = true;

                }
            }

        }
    }

    private void HeardPlayer(Vector3 soundLocation)
    {

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

    private void OnPlayerHide(Transform hidingPlace)
    {

    }
}


