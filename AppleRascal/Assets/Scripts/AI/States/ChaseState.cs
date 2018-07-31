using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class ChaseState : State
{

    public bool chasing = true;
    public bool goToPatrol;
    public bool justTurning = false;
    public Transform target;
  
  

    public ChaseState(AI _owner)
           : base()
    {
        _state = AIStateType.Chase;
        Owner = _owner;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        
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
                if (justTurning)
                {
                    Debug.Log("turning");
                    Vector3 targetDir = new Vector3(target.position.x, Owner.transform.position.y, target.position.z) - Owner.transform.position;

                    Owner.transform.rotation = Quaternion.LookRotation(targetDir, Owner.transform.up);
                    // Owner.transform.LookAt(Owner.target);
                }
                else
                {
                    Owner.Mover.Turn(target.position);
                    Owner.Mover.Move(target.forward);
                }

            }
        }
    }

    public void ChasePlayer()
    {
        if (Vector3.Distance(target.position, target.position) > Owner.giveUpChaseDistance)
        {
            StopChase();
        }
        if (Vector3.Distance(target.position, target.position) < Owner.attackDistance)
        {
            justTurning = true;
         //   Owner._turnSpeed += Owner._turnSpeedChaseModifier;
            if (Owner.fieldOfView.visibleTargets.Contains(target))
            {
                Owner.StartCoroutine(Owner.Attack());
            }

        }
        else
            justTurning = false;
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
