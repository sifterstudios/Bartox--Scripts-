using UnityEngine;

namespace Bartox.Bart.State
{
    
    public enum States
    {
        Locomotion,
        Interacting,
        Aiming,
        Strafing
    } 
    public class StateManager : MonoBehaviour
    {
        public static StateManager Singleton;

        void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else if (Singleton != this)
            {
                Destroy(gameObject);
            }
        }


        public States Current;

        public void TransitionToState(States newState)
        {
            var tmpInitialState = Current;
            OnStateExit(tmpInitialState, newState);
            Current = newState;
            OnStateEnter(newState, tmpInitialState);
        }
        
        public void OnStateEnter(States state, States fromState)
        {
            switch (state)
            {
                case States.Locomotion:
                {
                    break;
                }
            }
        }
        
        
        public void OnStateExit(States state, States toState)
        {
            switch (state)
            {
                case States.Locomotion:
                {
                    break;
                }
            }
        }
    }
}