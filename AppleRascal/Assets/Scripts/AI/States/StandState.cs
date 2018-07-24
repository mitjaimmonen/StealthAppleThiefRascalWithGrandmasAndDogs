using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStateStuff;

public class StandState : State
{      

    public override void Enter()
    {
        Debug.Log("Entering Stand state");
    }

    public override void Execute()
    {
      
    }

    public override void Exit()
    {
        Debug.Log("Exiting Stand state");
    }
}
