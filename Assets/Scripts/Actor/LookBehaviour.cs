using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class LookBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 10;

    [SerializeField]
    private UnityEvent<float> _onAngleChangeEvent;

    private Vector2 _lookDirection;
    private Vector3 _targetPosition;
    private Quaternion _rotation;
    private Quaternion _initialRotation;
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

    private void Start()
    {
        _initialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        _lookDirection = transform.forward.x_z().normalized;
        _rotation = transform.rotation;
    }

    private void LateUpdate()
    {
        Vector3 direction = _useDirection ? _lookDirection.x_z() : (_targetPosition - transform.position.x_z()).normalized;
         
        _rotation = Quaternion.Slerp(_rotation, _initialRotation * Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * _rotationSpeed);

        float angle = Quaternion.Dot(transform.rotation, _rotation);
        _onAngleChangeEvent.Invoke(angle);
    }
}
