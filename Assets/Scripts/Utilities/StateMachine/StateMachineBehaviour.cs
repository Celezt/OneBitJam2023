using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class StateMachineBehaviour : MonoBehaviour
{
    public StateBehaviour CurrentState
    {
        get => _currentState;
        set
        {
            var previousState = _currentState;
            var newState = value;

            _currentState = newState;

            if (newState != previousState)
            {
                if (previousState != null)
                    previousState.OnTransitionExit.Invoke();

                if (newState != null)
                    newState.OnTransitionEnter.Invoke();
            }
        }
    }

    [SerializeField]
    private StateBehaviour _defaultState;

    private StateBehaviour _currentState;

    private void Start()
    {
        _currentState = _defaultState;

        if (_currentState != null)
            _currentState.OnTransitionEnter.Invoke();
    }
}
