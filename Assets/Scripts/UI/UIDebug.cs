using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIDebug : MonoBehaviour
{
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private bool _isActiveByDefault = false;
    [SerializeField]
    private TextMeshProUGUI _velocityText;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _container?.SetActive(_isActiveByDefault);

        _playerInput = PlayerInput.GetPlayerByIndex(0);
        _rigidbody = _playerInput.GetComponentInParent<Rigidbody>();
    }

    private void OnEnable()
    {
        InputSystem.actions["Debug"].started += OnDebug;
    }

    private void OnDisable()
    {
        InputSystem.actions["Debug"].started -= OnDebug;
    }

    private void FixedUpdate()
    {
        if (_velocityText != null && _rigidbody != null)
            _velocityText.text = _rigidbody.velocity.magnitude.ToString("0.00") + " m/s";
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
