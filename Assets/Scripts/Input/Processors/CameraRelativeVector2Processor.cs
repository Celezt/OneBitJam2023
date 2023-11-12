using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class CameraRelativeVector2Processor : InputProcessor<Vector2>
{
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        var camera = Camera.main;
        float cameraAngle = camera.transform.eulerAngles.y;
        Vector3 relativeDirection = Quaternion.Euler(0, cameraAngle, 0) * value.x_z();
        return new Vector2(relativeDirection.x, relativeDirection.z);
    }

#if UNITY_EDITOR
    static CameraRelativeVector2Processor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<CameraRelativeVector2Processor>();
    }
}
