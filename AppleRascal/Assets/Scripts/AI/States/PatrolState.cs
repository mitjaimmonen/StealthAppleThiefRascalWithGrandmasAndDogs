using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class PatrolState : State
{
    private static PatrolState _instance;

    private Path _path;
    private Direction _direction;
    private float _arriveDistance;


    public Waypoint CurrentWaypoint { get; private set; }

    private bool Sentry
    {
        get
        {
            return Owner.sentry;
        }
    }

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


    private bool ChangeState()
    {
        return false;
        //add transitions
    }
}