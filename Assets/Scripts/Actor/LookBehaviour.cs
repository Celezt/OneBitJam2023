using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _rotor;
    [SerializeField]
    private bool _fixedRotation;
    [SerializeField, EnableIf(nameof(_fixedRotation)), Indent]
    private Quaternion _initialRotation;

    private Vector2 _lookDirection;

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            if (direction != Vector2.zero)
                Look(direction);
        }
    }

    public void Look(Vector2 direction)
    {
        _lookDirection = direction.normalized;
    }

    private void OnEnable()
    {
        _lookDirection = _rotor.forward.x_z().normalized;

        if (!_fixedRotation)
            _initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        _rotor.rotation = _initialRotation * Quaternion.LookRotation(_lookDirection.x_z(), Vector3.up);
    }
}
