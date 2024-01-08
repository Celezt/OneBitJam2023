using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class TargetBehaviour : MonoBehaviour
{
    [ShowInInspector, HideInEditorMode, PropertyOrder(-1), LabelText("Look Type (Runtime Only)")]
    public LookTypes LookType
    {
        get => _lookType;
        set => _lookType = value;
    }

    [SerializeField, Space(8)]
    private Transform _target;
    [SerializeField, MinMaxSlider(-180, 180, true)]
    private Vector2Int _minMaxLimit = new Vector2Int(-180, 180);
    [SerializeField, TitleGroup("Target Settings"), MinValue(0)]
    private float _deadZoneRadius = 1.0f;
    [SerializeField, TitleGroup("Direction Settings")]
    private bool _isDirectionWorldSpace = true;

    [SerializeField, Space(8)]
    private UnityEvent<Vector3> _onExceedingAngleEvent;

    private Vector3 _initialPosition;
    private Vector3 _worldPosition;
    private Vector3 _direction;
    private LookTypes _lookType = LookTypes.Direction;
    private float _directionDistance;

    public enum LookTypes
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
        _target.position = (transform.position + _direction * _directionDistance).x_z(_initialPosition.y);
        _lookType = LookTypes.Direction;
    }

    public void LookAt(InputAction.CallbackContext context)
        => LookAt(context.ReadValue<Vector2>());
    public void LookAt(Vector2 target)
        => LookAt(target.x_z(_initialPosition.y));
    public void LookAt(Vector3 target)
    {
        _worldPosition = target;
        _lookType = LookTypes.Target;
    }

    private void Start()
    {
        _initialPosition = _target.position;
        _direction = transform.forward;
        _directionDistance = Vector3.Distance(_initialPosition.x_z(), transform.position.x_z());
    }

    private void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 position = transform.position;
        Vector3 targetPosition = _target.position;

        switch (_lookType)
        {
            case LookTypes.Target:
                Vector3 direction = _worldPosition.x_z() - position.x_z();

                if (direction.magnitude < _deadZoneRadius)
                    _target.position = position.x_z(_worldPosition.y) + direction.normalized * _deadZoneRadius;
                else
                    _target.position = _worldPosition;
                break;
            case LookTypes.Direction when _isDirectionWorldSpace:
                _target.position = (position + _direction * _directionDistance).x_z(_initialPosition.y);
                break;
        }

        Vector3 targetDirection = (targetPosition.x_z() - position.x_z()).normalized;
        float angle = Vector2.SignedAngle(forward.xz().normalized, targetDirection.xz());

        if (!_minMaxLimit.Inside(angle))
            _onExceedingAngleEvent.Invoke(targetDirection);
    }
}
