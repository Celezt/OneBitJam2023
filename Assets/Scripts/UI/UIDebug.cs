using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class UIDebug : MonoBehaviour
{
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private bool _isActiveByDefault = false;
    [SerializeField]
    private TextMeshProUGUI _velocityText;
    [SerializeField]
    private InputAction _toggleDebugAction;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _container?.SetActive(_isActiveByDefault);

        _playerInput = PlayerInput.GetPlayerByIndex(0);

        if (_playerInput != null)
        {
            _rigidbody = _playerInput.GetComponentInParent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        _toggleDebugAction.Enable();

        if (_toggleDebugAction != null)
            _toggleDebugAction.started += OnDebug;
    }

    private void OnDisable()
    {
        _toggleDebugAction.Disable();

        if (_toggleDebugAction != null)
            _toggleDebugAction.started -= OnDebug;
    }

    private void FixedUpdate()
    {
        if (_velocityText != null && _rigidbody != null)
            _velocityText.text = _rigidbody.linearVelocity.magnitude.ToString("0.00") + " m/s";
    }

    private void OnDebug(InputAction.CallbackContext context)
    {
        if (_container == null)
            return;

        if (_container.activeSelf)
            _container.SetActive(false);
        else
            _container.SetActive(true);
    }
}
