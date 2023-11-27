using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookBehaviour : MonoBehaviour
{
    private static readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);

    public bool UseDirection
    {
        get => _useDirection;
        set => _useDirection = value;
    }
    public Vector3 TargetPosition => _targetPosition;
    public Vector2 LookDirection => _lookDirection;

    [SerializeField]
    private Transform _rotor;
    [SerializeField]
    private float _rotationSpeed = 10;
    [SerializeField]
    private bool _fixedRotation;
    [SerializeField, EnableIf(nameof(_fixedRotation)), Indent]
    private Quaternion _initialRotation;

    private Vector2 _lookDirection;
    private Vector3 _targetPosition;
    private Quaternion _rotation;
    private bool _useDirection = true;

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
            Look(context.ReadValue<Vector2>());
    }
    public void Look(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        _useDirection = true;
        _lookDirection = direction.normalized;
    }

    public void LookAt(InputAction.CallbackContext context)
        => LookAt(context.ReadValue<Vector2>());
    public void LookAt(Vector2 target)
        => LookAt(target.x_z());
    public void LookAt(Vector3 target)
    {
        _useDirection = false;
        _targetPosition = target;
    }

    private void OnEnable()
    {
        _lookDirection = _rotor.forward.x_z().normalized;
        _rotation = _rotor.rotation;

        if (!_fixedRotation)
            _initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        Vector3 direction = _useDirection ? _lookDirection.x_z() : (_targetPosition - transform.position.x_z()).normalized;

        _rotation = Quaternion.Slerp(_rotation, _initialRotation * Quaternion.LookRotation(direction, Vector3.up), 
                                            Time.deltaTime * _rotationSpeed);

        _rotor.rotation = _rotation;
    }
}
