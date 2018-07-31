using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class PatrolState : State
{
    private static PatrolState _instance;

    private Path _path;
    Transform Target;
    private Direction _direction;
    private float _arriveDistance;
    private bool goToChase;
    private bool goToCautious;
   

    public Waypoint CurrentWaypoint { get; private set; }

 

    public PatrolState(AI _owner, Path path, Direction direction, float arriveDistance) : base()
    {
        _state = AIStateType.Patrol;
        Owner = _owner;
        AddTransition(AIStateType.Stand);
        AddTransition(AIStateType.Cautious);
        AddTransition(AIStateType.Chase);
        _path = path;
        _direction = direction;
        _arriveDistance = arriveDistance;

    }

    public override void Enter()
    {
        Debug.Log("Entering Patrol state");
        CurrentWaypoint = _path.GetClosestWaypoint(Owner.transform.position);
        Owner.fieldOfView.detectEvent += OnDetection;

        goToCautious = false;
        goToChase = false;

    }

    public override void Execute()
    {
        if (!ChangeState())
        {
            if (!Sentry)
            {
                CurrentWaypoint = GetWaypoint();
                Owner.Mover.Move(Owner.transform.forward);
                Owner.Mover.Turn(CurrentWaypoint.Position);
            }
        }
    }

    private Waypoint GetWaypoint()
    {
        Waypoint result = CurrentWaypoint;
        Vector3 toWaypointVector = CurrentWaypoint.Position - Owner.transform.position;
        float toWaypointSqr = toWaypointVector.sqrMagnitude;
        float sqrArriveDistance = _arriveDistance * _arriveDistance;
        if (toWaypointSqr <= sqrArriveDistance)
        {
            if (!CurrentWaypoint.overrideSentry)
            {
                Owner.StartCoroutine(Owner.Sentry());
            }
            else
            {
                Owner.StartCoroutine(Owner.OverridenSentry(CurrentWaypoint));
            }
            result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
        }

        return result;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Patrol state");
    }

    public void OnDetection(Transform target)
    {

        if (target.gameObject.tag == ("Player"))
        {
            if (Vector3.Distance(Owner.transform.position, target.position) <= Owner.distanceToChase)
            {
                
                goToChase = true;
                Target = target;
            }
            else
            {

                goToCautious = true;
                Target = target;

            }
        }
        else if (target.gameObject.tag == "Trail" && !goToChase)
        {
            if (target.GetComponent<TrailPoint>().trailPointType == Owner.detectable)
            {
                goToCautious = true;
                Target = target;
            }
        }
    }

    private bool ChangeState()
    {
        if (goToChase)
        {            
            return Owner.stateMachine.PerformTransition(AIStateType.Chase);
        }
        else if (goToCautious)
        {
            Owner.target = Target;
            return Owner.stateMachine.PerformTransition(AIStateType.Cautious);
        }

        else
        {
            return false;
        }

    }

}