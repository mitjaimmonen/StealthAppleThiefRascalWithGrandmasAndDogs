using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class ChaseState : State
{

    public bool chasing = true;
    public bool goToPatrol;

    public ChaseState(AI _owner)
           : base()
    {
        _state = AIStateType.Chase;
        Owner = _owner;
        AddTransition(AIStateType.Patrol);  

    }

    public override void Enter()
    {
        Debug.Log("entered chase state");

        Owner._moveSpeed += Owner._moveSpeedChaseModifier;
        Owner.Mover.UpdateSpeed(Owner._moveSpeed);

    }

    public override void Execute()
    {
        if (!ChangeState())
        {
            if (!Sentry)
            {
                ChasePlayer();
                Owner.Mover.Turn(Owner.target.position);
                Owner.Mover.Move(Owner.transform.forward);

            }
        }
    }

    public void ChasePlayer()
    {
        if (Vector3.Distance(Owner.target.position, Owner.transform.position) > Owner.giveUpChaseDistance)
        {
            StopChase();
        }
        if (Vector3.Distance(Owner.target.position, Owner.transform.position) < Owner.attackDistance)
        {
            Owner.StartCoroutine(Owner.Attack());
        }
    }

    private void StopChase()
    {
        goToPatrol = true;
    }

    public override void Exit()
    {
        Owner._moveSpeed -= Owner._moveSpeedChaseModifier;
        Owner.Mover.UpdateSpeed(Owner._moveSpeed);

        Owner.StartCoroutine(Owner.SentryForFollowing(180, 2.5f));
    }

    private void OnPlayerHide(Transform hidingPlace)
    {
        StopChase();
    }


    public bool ChangeState()
    {
        if (goToPatrol)
        {
            return Owner.stateMachine.PerformTransition(AIStateType.Patrol);
        }
        else
            return false;
    }
}
