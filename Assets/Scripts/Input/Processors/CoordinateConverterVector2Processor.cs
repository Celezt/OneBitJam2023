using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEngine.InputSystem.Editor;
using UnityEngine.UIElements;
#endif

namespace UnityEngine.InputSystem
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class CoordinateConverterVector2Processor : InputProcessor<Vector2>
    {
        private static Camera _camera;

        public int Coordinate;

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            switch (Coordinate)
            {
                case 1: // Camera.
                    if (!_camera)
                    {
                        _camera = Camera.main;

                        if (!_camera)
                            goto default;
                    }

                    float cameraAngle = _camera.transform.eulerAngles.y;
                    Vector3 relativeDirection = Quaternion.Euler(0, cameraAngle, 0) * value.x_z();
                    return new Vector2(relativeDirection.x, relativeDirection.z).normalized;
                case 0: // World.
                default:
                    return value.normalized;
            }
        }

#if UNITY_EDITOR
        static CoordinateConverterVector2Processor()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<CoordinateConverterVector2Processor>();
        }
    }

#if UNITY_EDITOR
    public class CoordinateConverterVector2Editor : InputParameterEditor<CoordinateConverterVector2Processor>
    {
        private static List<string> _options = new()
    {
        "World",
        "Camera",
    };

        public override void OnGUI()
        {

        }

        public override void OnDrawVisualElements(VisualElement root, Action onChangedCallback)
        {
            var coordinateField = new PopupField<string>(_options, target.Coordinate);
            coordinateField.RegisterValueChangedCallback(x =>
            {
                target.Coordinate = _options.IndexOf(x.newValue);
                onChangedCallback.Invoke();
            });
            root.Add(coordinateField);
        }
    }
#endif
}
