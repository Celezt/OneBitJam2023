using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class LookBehaviour : MonoBehaviour
{
    private const int DIRECTION_DISTANCE = 4;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private bool _isDirectionWorldSpace;

    private Vector3 _initialPosition;
    private Vector3 _worldPosition;
    private Vector3 _direction;
    private LookType _lookType;

    private enum LookType
    {
        Direction,
        Target,
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
            Look(context.ReadValue<Vector2>());
    }
    public void Look(Vector2 direction)
        => Look(direction.x_z());
    public void Look(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        _direction = direction.normalized;
        _target.position = (transform.position + _direction * DIRECTION_DISTANCE).x_z(_initialPosition.y);
        _lookType = LookType.Direction;
    }

    public void LookAt(InputAction.CallbackContext context)
        => LookAt(context.ReadValue<Vector2>());
    public void LookAt(Vector2 target)
        => LookAt(target.x_z(_initialPosition.y));
    public void LookAt(Vector3 target)
    {
        _worldPosition = target;
        _lookType = LookType.Target;
    }

    private void Start()
    {
        _initialPosition = _target.position;
    }

    private void LateUpdate()
    {
        switch (_lookType)
        {

            case LookType.Target:
                _target.position = _worldPosition;
                break;
            case LookType.Direction when _isDirectionWorldSpace:
                _target.position = (transform.position + _direction * DIRECTION_DISTANCE).x_z(_initialPosition.y);
                break;
        }
    }
}
