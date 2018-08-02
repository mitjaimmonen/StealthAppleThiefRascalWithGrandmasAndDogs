using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;
using WaypointSystem;

public class ChaseState : State
{

    public bool goToPatrol;
    public bool justTurning = false;
    public bool playerHiding;
    public Transform target;
    float lookForCounter;
    public Transform player;



    public ChaseState(AI _owner)
           : base()
    {
        _state = AIStateType.Chase;
        Owner = _owner;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        AddTransition(AIStateType.Patrol);

    }

    public override void Enter()
    {
        Debug.Log("entered chase state");
        Owner.navMeshAgent.speed += Owner._moveSpeedChaseModifier;
        GameMaster.Instance.OnHide += OnPlayerHid;
        goToPatrol = false;
        playerHiding = false;
        GameMaster.Instance.OnAlertMode(player);
        lookForCounter = 0;
        if (!player.GetComponent<Player>().IsInvisible)
        {
            target = player;
        }

        Owner.StartCoroutine(Owner.DetectDelay(0.3f,_state));

    }

    public override void Execute()
    {
        if (!ChangeState())
        {
            Debug.Log(Owner.gameObject.name + " is on Chase mode");

            if (!Sentry)
            {
                if (!playerHiding)
                {
                    ChasePlayer();

                    if (justTurning)
                    {
                        Vector3 targetDir = new Vector3(target.position.x, Owner.transform.position.y, target.position.z) - Owner.transform.position;
                        Owner.transform.rotation = Quaternion.LookRotation(targetDir, Owner.transform.up);
                    }
                    else
                    {
                        Owner.navMeshAgent.SetDestination(target.position);
                        Owner.Mover.Turn(target.position);
                    }
                }

                else
                {
                    CheckOutHidingSpot();
                }
            }
            if (Sentry)
                lookForCounter += Time.deltaTime;
            if (lookForCounter >= 2.5)
                goToPatrol = true;
        }
    }

    public void ChasePlayer()
    {
        if (Vector3.Distance(Owner.transform.position, target.position) > Owner.giveUpChaseDistance)
        {
            //  StopChase();
            justTurning = false;
        }
        if (Vector3.Distance(Owner.transform.position, target.position) < Owner.attackDistance)
        {
            justTurning = true;
            if (Owner.fieldOfView.visibleTargets.Contains(target))
            {
                Owner.StartCoroutine(Owner.Attack());
            }

        }
        else
        {
            justTurning = false;
        }

    }

    private void CheckOutHidingSpot()
    {
        Debug.Log("distance to " + target.name + " is " + Vector3.Distance(Owner.transform.position, target.position));

        if (Vector3.Distance(Owner.transform.position, target.position) >= 5f)
        {
            Debug.Log("got here, target is " + target.name);
            Owner.Mover.Turn(target.position);
            Owner.navMeshAgent.SetDestination(target.position);
        }
        else
        {
            Owner.navMeshAgent.isStopped = true;
            Debug.Log("gave up looking for racoon");
            if (!Sentry)
                Owner.StartCoroutine(Owner.SentryForFollowing(180, 2.5f));         
        }
    }

    private void StopChase()
    {
        goToPatrol = true;
    }

    private void OnPlayerHid(Transform hidingSpot, bool toggle)
    {
        target = hidingSpot;
        playerHiding = toggle;
    }

    public override void Exit()
    {
        Owner.navMeshAgent.speed -= Owner._moveSpeedChaseModifier;
        GameMaster.Instance.OnHide -= OnPlayerHid;
        //Owner.StartCoroutine(Owner.SentryForFollowing(180, 2.5f));
        GameMaster.Instance.setAlertMode(false);
    }

    public bool ChangeState()
    {
        if (!Sentry)
        {
            if (goToPatrol)
            {
                Debug.Log("going from chase to patrol");
                return Owner.stateMachine.PerformTransition(AIStateType.Patrol);
            }
            else
                return false;
        }
        else
            return false;
    }
}
