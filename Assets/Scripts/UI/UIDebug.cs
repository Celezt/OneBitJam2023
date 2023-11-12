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
    private TextMeshProUGUI _velocityText;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _playerInput = PlayerInput.GetPlayerByIndex(0);
        _rigidbody = _playerInput.GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_velocityText != null && _rigidbody != null)
            _velocityText.text = _rigidbody.velocity.magnitude.ToString("0.00") + " m/s";
    }
}
