using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionVector2Processor : IProcessor<Vector2>
{
    private static readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);

    private Camera _camera;

    public Vector2 Process(Vector2 value)
    {
        _camera ??= Camera.main;
        Ray ray = _camera.ScreenPointToRay(value);

        if (_plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter).xz();

        return value;
    }
}
