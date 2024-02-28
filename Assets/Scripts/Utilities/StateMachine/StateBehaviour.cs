using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class StateBehaviour : MonoBehaviour
{
    public UnityEvent OnTransitionEnter => _onTransitionEnter;
    public UnityEvent OnTransitionExit => _onTransitionExit;

    [SerializeField]
    private StateMachineBehaviour _stateMachine;

    [SerializeField, Space(8)]
    private UnityEvent _onTransitionEnter;
    [SerializeField]
    private UnityEvent _onTransitionExit;

    public void TransitionToState()
    {
        _stateMachine.CurrentState = this;
    }

    private void OnValidate()
    {
        if (_stateMachine == null)
            _stateMachine = GetComponentInParent<StateMachineBehaviour>();
    }
}
