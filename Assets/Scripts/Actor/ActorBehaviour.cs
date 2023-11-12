using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorBehaviour : MonoBehaviour
{
    /// <summary>
    /// Direction in world space.
    /// </summary>
    public Vector3 Direction => _direction;
    public float MoveForce
    {
        get => _moveForce;
        set => _moveForce = value;
    }
    public Coordinates Coordinate
    {
        get => _coordinate;
        set => _coordinate = value;
    }

    [SerializeField] 
    private Rigidbody _rigidbody;
    [SerializeField]
    private TriggerHandler _trigger;
    [SerializeField]
    private float _moveForce = 40f;
    [SerializeField]
    private float _dragCoefficientHorizontal = 4f;
    [SerializeField]
    private AnimationCurve _initialForceCurve = AnimationCurve.EaseInOut(0, 20, 1, 0);
    [SerializeField]
    private AnimationCurve _stopForceCurve = AnimationCurve.EaseInOut(0, 0, 1, 20);
    [SerializeField]
    private Coordinates _coordinate = Coordinates.World;

    private CancellationTokenSource _cancellationTokenSource;
    private Vector3 _direction;
    private bool _isMoving;

    public enum Coordinates
    {
        World,
        Camera,
    }

    public void Move(InputAction.CallbackContext context)
        => Move(context.ReadValue<Vector2>());

    public void Move(Vector2 direction)
    {
        direction.Normalize();

        switch (_coordinate)
        {
            case Coordinates.World:
                _direction = direction.x_z();
                break;
            case Coordinates.Camera:
                var camera = Camera.main;
                float cameraAngle = camera.transform.eulerAngles.y;
                Vector3 relativeDirection = Quaternion.Euler(0, cameraAngle, 0) * direction.x_z();
                _direction = relativeDirection;
                break;
        }

        if (_direction == Vector3.zero)
        {
            _isMoving = false;
        }
        else
        {
            if (!_isMoving)
            {
                CTSUtility.Reset(ref _cancellationTokenSource);
                InitialForceAsync(_cancellationTokenSource.Token).Forget();
            }

            _isMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (!_trigger?.IsTriggered ?? false)  // Don't move if it actor is not on the ground.
        {
            _direction = Vector3.zero;
            return;
        }

        Vector3 dragForce = _direction != Vector3.zero ? 
            -_dragCoefficientHorizontal * _rigidbody.velocity.x_z() : Vector3.zero;
        Vector3 totalMoveForce = (_direction * _moveForce) + dragForce;

        _rigidbody?.AddForce(totalMoveForce);
    }

    private void OnDestroy()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private async UniTaskVoid InitialForceAsync(CancellationToken cancellationToken)
    {
        if (_initialForceCurve.length == 0) // There is no keys.
            return;

        float maxDuration = _initialForceCurve[_initialForceCurve.length - 1].time;
        float startTime = Time.time;

        while (!cancellationToken.IsCancellationRequested)
        {
            float currentTime = Time.time;
            float duration = currentTime - startTime;

            if (duration >= maxDuration)
                break;

            float force = _initialForceCurve.Evaluate(duration);
            _rigidbody.AddForce(_direction * force);

            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }
}
