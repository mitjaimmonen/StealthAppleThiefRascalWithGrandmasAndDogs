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
    }

    public void OnDetection(Transform target)
    {
        if (currentTarget == target)
        {
            return;
        }

        if (target.tag == "Player")
        {
            goToChase = true;
        }

        if (followingTrail)
        {
            return;
        }
        else
        {
            if (target.tag == "Trail")
            {
                currentTarget = target;
                followingTrail = true;
            }
        }
    }

    private void FollowTrail()
    {
        if (currentTarget.gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(Owner.transform.position, currentTarget.position) <= 1.5f)
            {
                followingTrail = false;
                lastTarget = currentTarget;
                currentTarget = null;
                //look for next trail piece

                if (lastTarget.tag == "Trail")
                {
                    TrailPoint trailPoint = lastTarget.gameObject.GetComponent<TrailPoint>();
                    if (trailPoint.connectedTrailPoint && trailPoint.connectedTrailPoint.enabled)
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
            followingTrail = false;
        }
    }

    public override void Execute()
    {
        Debug.Log("follow trail is: " + followingTrail);
        if (!ChangeState())
        {
            if (!Owner.sentry && followingTrail)
            {
                Owner.Mover.Turn(currentTarget.position);
                Owner.Mover.Move(Owner.transform.forward);
                FollowTrail();
                lookForCounter = 0;
            }
            else if (!Owner.sentry && !followingTrail)
            {
                Debug.Log("starting sentry for caution");
                Owner.SentryForFollowing(0, 1);
                goToPatrol = true;
            }
            else
            {
                lookForCounter += Time.deltaTime;
            }
            if (lookForCounter >= 2)
            {
                goToPatrol = true;
            }
        }
    }



    public override void Exit()
    {

    }

    private bool ChangeState()
    {
        if (goToChase)
        {
            return false;
        }
        if (goToPatrol)
        {
            Debug.Log("got here");
            return Owner.stateMachine.PerformTransition(AIStateType.Patrol);
        }

        return false;

    }
}


