using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class InputSystemBinder : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _inputActionReference;

    [SerializeField, Space(8)]
    private UnityEvent<InputAction.CallbackContext> _onStartedEvent;
    [SerializeField]
    private UnityEvent<InputAction.CallbackContext> _onPerformedEvent;
    [SerializeField]
    private UnityEvent<InputAction.CallbackContext> _onCancelledEvent;

    private void OnEnable()
    {
        if (_onStartedEvent.GetPersistentEventCount() > 0) 
            _inputActionReference.action.started += OnStarted;

        if (_onPerformedEvent.GetPersistentEventCount() > 0)
            _inputActionReference.action.performed += OnPerformed;

        if (_onCancelledEvent.GetPersistentEventCount() > 0)
            _inputActionReference.action.canceled += OnCancelled;
    }

    private void OnDisable()
    {
        _inputActionReference.action.started -= OnStarted;
        _inputActionReference.action.performed -= OnPerformed;
        _inputActionReference.action.canceled -= OnCancelled;
    }

    private void OnStarted(InputAction.CallbackContext context)
    {
        _onStartedEvent.Invoke(context);
    }

    private void OnPerformed(InputAction.CallbackContext context)
    {
        _onPerformedEvent.Invoke(context);
    }

    private void OnCancelled(InputAction.CallbackContext context)
    {
        _onCancelledEvent.Invoke(context);
    }
}
