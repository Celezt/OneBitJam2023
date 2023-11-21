using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ScreenToWorldPosition : MonoBehaviour
{
    private static readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);

    public UnityEvent<Vector2> _onPositionChangedEvent;

    private Camera _camera;
     

    public void OnScreenPosition(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(screenPosition);

        if (!_plane.Raycast(ray, out float enter))
            return;

        Vector3 targetWorldPosition = ray.GetPoint(enter);
        _onPositionChangedEvent.Invoke(targetWorldPosition.xz());
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }
}
