using System.Collections;
using System.Collections.Generic;
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
    private float _dragCoefficientHorizontal = 4f;
    [SerializeField]
    private float _moveForce = 40f;
    [SerializeField]
    private Coordinates _coordinate = Coordinates.World;

    private Vector3 _direction;

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
}
