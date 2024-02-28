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
    private StateBehaviour _firstState;
    [SerializeField]
    private StateBehaviour _secondState;

    public void Switch()
    {
        if (_stateMachine.CurrentState == _firstState && _firstState != null)
            _stateMachine.CurrentState = _secondState;
        else if (_stateMachine.CurrentState == _secondState && _secondState != null)
            _stateMachine.CurrentState = _firstState;
    }

    private void OnValidate()
    {
        if (_stateMachine == null)
            _stateMachine = GetComponentInParent<StateMachineBehaviour>();
    }
}
