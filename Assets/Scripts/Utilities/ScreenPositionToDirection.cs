using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ScreenPositionToDirection : MonoBehaviour
{
    private static readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);

    [SerializeField]
    private Transform _pivot;

    public UnityEvent<Vector2> _onDirectionEvent;

    private Camera _camera;
     

    public void OnScreenPosition(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(screenPosition);

        if (!_plane.Raycast(ray, out float enter))
            return;

        Vector3 targetWorldPosition = ray.GetPoint(enter);
        Vector3 pivotWorldPosition = _plane.ClosestPointOnPlane(_pivot.position);
        Vector2 direction = targetWorldPosition.xz() - pivotWorldPosition.xz();

        _onDirectionEvent.Invoke(direction);
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }
}
