using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CoordinateConverter2D : MonoBehaviour
{
    public InputStates InputState
    {
        get => _inputState;
        set => _inputState = value;
    }
    public Coordinates Coordinate
    {
        get => _coordinate;
        set => _coordinate = value;
    }

    [SerializeField]
    private InputStates _inputState = InputStates.All;
    [SerializeField]
    private Coordinates _coordinate = Coordinates.World;

    public UnityEvent<Vector2> OnConvertedEvent;

    private Camera _camera;

    [Flags]
    public enum InputStates
    {
        Started = 1 << 1,
        Performed = 1 << 2,
        Canceled = 1 << 3,
        All = Started | Performed | Canceled
    }

    public enum Coordinates
    {
        World,
        Camera,
    }

    public void Convert(InputAction.CallbackContext context)
    {
        if (_inputState.HasFlag(InputStates.Started) && context.started)
            Convert(context.ReadValue<Vector2>());
        else if (_inputState.HasFlag(InputStates.Performed) && context.performed)
            Convert(context.ReadValue<Vector2>());
        else if (_inputState.HasFlag(InputStates.Canceled) && context.canceled)
            Convert(context.ReadValue<Vector2>());
    }

    public void Convert(Vector2 direction)
    {
        switch (_coordinate)
        {
            case Coordinates.World:
                OnConvertedEvent.Invoke(direction.normalized);
                break;
            case Coordinates.Camera:
                float cameraAngle = _camera.transform.eulerAngles.y;
                Vector3 relativeDirection = Quaternion.Euler(0, cameraAngle, 0) * direction.x_z();
                OnConvertedEvent.Invoke(relativeDirection.xz());
                break;
        }
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }
}
