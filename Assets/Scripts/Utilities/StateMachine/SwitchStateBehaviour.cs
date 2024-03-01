using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class SwitchStateBehaviour : MonoBehaviour
{
    [SerializeField]
    private StateMachineBehaviour _stateMachine;

    [SerializeField, Space(4)]
    private StateBehaviour _defaultState;
    [SerializeField]
    private StateBehaviour _transitionState;

    public void Switch()
    {
        if (_stateMachine.CurrentState == _defaultState && _defaultState != null && _transitionState != null)
            _stateMachine.CurrentState = _transitionState;
        else if (_stateMachine.CurrentState != _defaultState)
            _stateMachine.CurrentState = _defaultState;
    }

    private void OnValidate()
    {
        if (_stateMachine == null)
            _stateMachine = GetComponentInParent<StateMachineBehaviour>();
    }
}
