using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AIStateStuff
{
    public class StateMachine
    {
        public State CurrentState;
        private State previousState;
       

        public AI Owner;

        private IList<State> _states = new List<State>();

        public StateMachine(AI owner_)
        {
            Owner = owner_;
            CurrentState = null;
            previousState = null;
        }

        public bool PerformTransition(AIStateType targetState)
        {
            if (CurrentState.CheckTransition(targetState))
            {
                return false;
            }

            bool result = false;

            State state = GetStateByType(targetState);
            if (state != null)
            {
                CurrentState.Exit();
                CurrentState = state;
                CurrentState.Enter();
                result = true;
            }

            return result;
        }

        private State GetStateByType(AIStateType stateType)
        {
            // Returns the first object from the list _states which State property's value
            // equals to stateType. If no object is found, returns null.
            return _states.FirstOrDefault(state => state._state == stateType);
          
        }
       
        public void InitStates()
        {
            PatrolState patrol = new PatrolState(Owner, Owner._path, Owner._direction, Owner._waypointArriveDistance);
            _states.Add(patrol);

            CurrentState = patrol;
            CurrentState.Enter();           

            //add the other states too
        }

        public void ExecuteStateUpdate()
        {
            //State<T> runningState = currentState;

            if (CurrentState != null)
            {
                CurrentState.Execute();
            }
        }


    }
}
