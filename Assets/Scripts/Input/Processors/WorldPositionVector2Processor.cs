using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.InputSystem
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class WorldPositionVector2Processor : InputProcessor<Vector2>
    {
        private static readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);
        private static Camera _camera;

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            if (!_camera)
                _camera = Camera.main;

            Ray ray = _camera.ScreenPointToRay(value);

            if (_plane.Raycast(ray, out float enter))
                return ray.GetPoint(enter).xz();

            return value;
        }

#if UNITY_EDITOR
        static WorldPositionVector2Processor()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<WorldPositionVector2Processor>();
        }
    }
}