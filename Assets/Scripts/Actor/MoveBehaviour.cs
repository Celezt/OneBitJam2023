using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MoveBehaviour : MonoBehaviour
{
    /// <summary>
    /// Direction in world space.
    /// </summary>
    public Vector3 Direction => _direction;
    /// <summary>
    /// Direction the actor should looking towards. Is never zero.
    /// </summary>
    public Vector3 LookDirection => _lookDirection;
    public bool IsMoving => _isMoving;
    public float MoveForce
    {
        get => _moveForce;
        set => _moveForce = value;
    }

    [SerializeField] 
    private Rigidbody _rigidbody;
    [SerializeField]
    private TriggerHandler _groundTrigger;
    [SerializeField]
    private float _rotationSpeed = 8f;
    [SerializeField]
    private float _moveForce = 40f;
    [SerializeField]
    private float _dragCoefficientHorizontal = 4f;
    [SerializeField]
    private AnimationCurve _dashCoefficientCurve = AnimationCurve.EaseInOut(0, 1, 0.5f, 0);
    [SerializeField, Range(0, 360)]
    private float _angleDifferenceToDash = 80;
    [SerializeField]
    private AnimationCurve _stopCoefficientCurve = AnimationCurveBuilder.EaseInOut(0, 0, 0.25f, 0.5f, 0.5f, 0);

    [SerializeField]
    private UnityEvent<float> _onSpeedChangeEvent;

    private CancellationTokenSource _dashForceCancellationTokenSource;
    private CancellationTokenSource _stopForceCancellationTokenSource;
    private UniTask.Awaiter _stopForceAwaiter;
    private Vector3 _direction;
    private Vector3 _lookDirection;
    private bool _isMoving;

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

        if (_isMoving)  // Only update when a direction exist.
            _lookDirection = _direction;
    }

    private void OnEnable()
    {
        _lookDirection = transform.forward;
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        if (_groundTrigger == null && _groundTrigger.IsTriggered)  // Don't move if it actor is not on the ground.
            _direction = Vector3.zero;

        Vector3 velocity = _rigidbody.velocity.x_z();

        Vector3 dragForce = _direction != Vector3.zero ? 
            -_dragCoefficientHorizontal * velocity : Vector3.zero;
        Vector3 totalMoveForce = (_direction * _moveForce) + dragForce;

        Quaternion lookRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
        Quaternion rotation = Quaternion.Slerp(_rigidbody.rotation, lookRotation, deltaTime * _rotationSpeed);
        _rigidbody.MoveRotation(rotation);

        _rigidbody.AddForce(totalMoveForce);

        float minSpeed = (totalMoveForce.magnitude / _rigidbody.mass) * deltaTime * 10.0f;
        float speed = _direction.IsZero() ? velocity.magnitude : Mathf.Max(velocity.magnitude, minSpeed);
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

        while (!cancellationToken.IsCancellationRequested)
        {
            float currentTime = Time.time;
            float duration = currentTime - startTime;

            if (duration >= maxDuration)
                break;

            if (_direction == Vector3.zero)
                startDirection = Vector3.zero;

            float force = _dashCoefficientCurve.Evaluate(duration) * _moveForce;
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

        while (!cancellationToken.IsCancellationRequested)
        {
            float currentTime = Time.time;
            float duration = currentTime - startTime;

            if (duration >= maxDuration)
                break;

            if (_direction != Vector3.zero)
                break;

            float force = _stopCoefficientCurve.Evaluate(duration) * _moveForce;
            _rigidbody.AddForce(-startDirection * force);

            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }
}
