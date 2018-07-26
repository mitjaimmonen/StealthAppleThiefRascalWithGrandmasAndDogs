using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIStateStuff
{
    public enum AIStateType
    {
        Error = 0,
        Patrol = 1,
        Stand = 2,
        Cautious = 3,
        Chase = 4
    }

    public abstract class State
    {      

        public AIStateType _state { get; protected set; }
        public IList<AIStateType> TargetStates { get; protected set; }
        public AI Owner { get; protected set; }
        
        protected State()
        {
            TargetStates = new List<AIStateType>();
        }

        protected State(AI owner, AIStateType state)
        : this()
        {
            Owner = owner;
            _state = state;
        }

        public bool AddTransition(AIStateType targetState)
        {           
            return TargetStates.AddUnique(targetState);
        }

        public bool RemoveTransition(AIStateType targetState)
        {
            return TargetStates.Remove(targetState);

        }

        public virtual bool CheckTransition(AIStateType targetState)
        {
            return TargetStates.Contains(targetState);
        }



        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();

    }
}
