using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[HideMonoScript]
public class MoveBehaviour : MonoBehaviour
{
    /// <summary>
    /// Direction in world space.
    /// </summary>
    public Vector3 Direction => _direction;
    /// <summary>
    /// Direction the actor should looking towards. Is never zero.
    /// </summary>
    [ShowInInspector, HideInEditorMode, PropertyOrder(-1), LabelText("Rotation (Runtime)")]
    public Quaternion LookRotation
    {
        get => _lookRotation;
        set => _lookRotation = value;
    }
    public bool IsMoving => _isMoving;
    /// <summary>
    /// Current move force affected by direction multipliers.
    /// </summary>
    public float CurrentMoveForce
    {
        get
        {
            float moveForce = _moveForce;

            Vector3 velocity = _rigidbody.velocity.x_z();
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float angle = Vector2.SignedAngle(localVelocity.xz().normalized, Vector2.up);

            if (_strafeMinMax.Outside(angle))
                moveForce *= _moveBackwardsMultiplier;
            else if (_forwardMinMax.Outside(angle))
                moveForce *= _moveStrafeMultiplier;

            return moveForce;
        }
    }
    public float MoveForce
    {
        get => _moveForce;
        set => _moveForce = value;
    }

    [SerializeField, Space(10)] 
    private Rigidbody _rigidbody;
    [SerializeField]
    private TriggerHandler _groundTrigger;
    [SerializeField]
    private float _rotationSpeed = 8f;
    [SerializeField]
    private float _moveForce = 40f;
    [SerializeField, LabelText("Rotate When Moving")]
    private bool _isRotateWhenMove = true;
    [SerializeField, Indent]
    private float _moveBackwardsMultiplier = 0.65f;
    [SerializeField, Indent]
    private float _moveStrafeMultiplier = 0.8f;
    [SerializeField, Indent, MinMaxSlider(-180, 180, true)]
    private Vector2Int _forwardMinMax = new Vector2Int(-45, 45);
    [SerializeField, Indent, MinMaxSlider(-180, 180, true)]
    private Vector2Int _strafeMinMax = new Vector2Int(-110, 110);
    [SerializeField]
    private float _dragCoefficientHorizontal = 4f;
    [SerializeField]
    private AnimationCurve _dashCoefficientCurve = AnimationCurve.EaseInOut(0, 1, 0.5f, 0);
    [SerializeField, Range(0, 360)]
    private float _angleDifferenceToDash = 80;
    [SerializeField]
    private AnimationCurve _stopCoefficientCurve = AnimationCurveBuilder.EaseInOut(0, 0, 0.25f, 0.5f, 0.5f, 0);

    [SerializeField, Space(8)]
    private UnityEvent<float> _onSpeedChangeEvent;
    [SerializeField]
    private UnityEvent<Vector2> _onMoveChangeEvent;

    private CancellationTokenSource _dashForceCancellationTokenSource;
    private CancellationTokenSource _stopForceCancellationTokenSource;
    private UniTask.Awaiter _stopForceAwaiter;
    private Vector3 _direction;
    private Quaternion _lookRotation;
    private bool _isMoving;

    public void SetDirection(Vector2 direction)
        => SetDirection(direction.x_z());
    public void SetDirection(Vector3 direction)
    {
        _lookRotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void Move(InputAction.CallbackContext context)
        => Move(context.ReadValue<Vector2>());

    public void Move(Vector2 direction)
    {
        _direction = direction.x_z().normalized;

        if (_direction != Vector3.zero)
        {
            float angle = Vector3.Angle(_rigidbody.transform.forward, _direction);
            if ((angle > _angleDifferenceToDash || !_isMoving) && _stopForceAwaiter.IsCompleted)
            {
                CTSUtility.Reset(ref _dashForceCancellationTokenSource);
                _stopForceAwaiter = DashForceAsync(_dashForceCancellationTokenSource.Token).GetAwaiter();
            }
        }
        else
        {
            CTSUtility.Reset(ref _stopForceCancellationTokenSource);
            StopForceAsync(_stopForceCancellationTokenSource.Token).Forget();
        }

        _isMoving = _direction != Vector3.zero;

        if (_isMoving && _isRotateWhenMove)  // Only update when a direction exist.
            _lookRotation = Quaternion.LookRotation(_direction, Vector3.up);
    }

    private void OnEnable()
    {
        _lookRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_groundTrigger == null && _groundTrigger.IsTriggered)  // Don't move if it actor is not on the ground.
            _direction = Vector3.zero;

        float deltaTime = Time.deltaTime;
        Vector3 velocity = _rigidbody.velocity.x_z();
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        Vector3 dragForce = _direction != Vector3.zero ? -_dragCoefficientHorizontal * velocity : Vector3.zero;

        Vector3 totalMoveForce = (_direction * CurrentMoveForce) + dragForce;

        Quaternion rotation = Quaternion.Slerp(_rigidbody.rotation, _lookRotation, deltaTime * _rotationSpeed);

        float minSpeed = (totalMoveForce.magnitude / _rigidbody.mass) * deltaTime * 10.0f;
        float speed = (float)Math.Round(_direction.IsZero() ? velocity.magnitude : Mathf.Max(velocity.magnitude, minSpeed), 2, MidpointRounding.AwayFromZero);

        _rigidbody.AddForce(totalMoveForce);
        _rigidbody.MoveRotation(rotation);

        _onMoveChangeEvent.Invoke(new Vector2(localVelocity.x, localVelocity.z));
        _onSpeedChangeEvent.Invoke(speed);
        
    }

    private void OnDestroy()
    {
        CTSUtility.Clear(ref _dashForceCancellationTokenSource);
        CTSUtility.Clear(ref _stopForceCancellationTokenSource);
    }

    private async UniTask DashForceAsync(CancellationToken cancellationToken)
    {
        if (_dashCoefficientCurve.length == 0)     // There is no keys.
            return;

        float maxDuration = _dashCoefficientCurve[_dashCoefficientCurve.length - 1].time;
        float startTime = Time.time;
        Vector3 startDirection = _direction;
        float moveForce = CurrentMoveForce;

        while (!cancellationToken.IsCancellationRequested)
        {
            float currentTime = Time.time;
            float duration = currentTime - startTime;

            if (duration >= maxDuration)
                break;

            if (_direction == Vector3.zero)
                startDirection = Vector3.zero;

            float force = _dashCoefficientCurve.Evaluate(duration) * moveForce;
            _rigidbody.AddForce(startDirection * force);

            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }

    private async UniTask StopForceAsync(CancellationToken cancellationToken)
    {
        if (_stopCoefficientCurve.length == 0)    // There is no keys.
            return;

        float maxDuration = _stopCoefficientCurve[_stopCoefficientCurve.length - 1].time;
        float startTime = Time.time;
        Vector3 startDirection = _rigidbody.velocity.x_z().normalized;
        float moveForce = CurrentMoveForce;

        while (!cancellationToken.IsCancellationRequested)
        {
            float currentTime = Time.time;
            float duration = currentTime - startTime;

            if (duration >= maxDuration)
                break;

            if (_direction != Vector3.zero)
                break;

            float force = _stopCoefficientCurve.Evaluate(duration) * moveForce;
            _rigidbody.AddForce(-startDirection * force);

            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }
}
