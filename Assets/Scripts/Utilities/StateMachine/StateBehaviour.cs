using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

[HideMonoScript]
public class StateBehaviour : MonoBehaviour
{
    public UltEvent OnTransitionEnter => _onTransitionEnter;
    public UltEvent OnTransitionExit => _onTransitionExit;

    [SerializeField]
    private StateMachineBehaviour _stateMachine;
    [SerializeField]
    private bool _exitOnStart = true;

    [SerializeField, Space(8)]
    private UltEvent _onTransitionEnter;
    [SerializeField]
    private UltEvent _onTransitionExit;

    public void TransitionToState()
    {
        _stateMachine.CurrentState = this;
    }

    private void OnValidate()
    {
        if (_stateMachine == null)
            _stateMachine = GetComponentInParent<StateMachineBehaviour>();
    }

    private void Start()
    {
        if (_exitOnStart && _stateMachine != null && _stateMachine.CurrentState != this)
            _onTransitionExit.Invoke();
    }
}
