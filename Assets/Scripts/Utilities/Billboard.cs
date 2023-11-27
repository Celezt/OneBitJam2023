using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class Billboard : MonoBehaviour
{
    private static Camera _camera;

    [SerializeField, SuffixLabel("Optional")]
    private Transform _target;
    [SerializeField]
    private bool _fixedRotation;
    [SerializeField, EnableIf(nameof(_fixedRotation)), Indent]
    private Quaternion _initialRotation;

    private Transform ActiveFacedObject
    {
        get
        {
            if (_target != null) 
                return _target;

            if (_camera != null) 
                return _camera.transform;

            _camera = Camera.main;

            return _camera == null ? null : _camera.transform;
        }
    }

    private void OnEnable()
    {
        if (!_fixedRotation)
            _initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (ActiveFacedObject == null) 
            return;

        float cameraAngle = ActiveFacedObject.transform.eulerAngles.y;
        Quaternion relativeRotation = _initialRotation * Quaternion.Euler(0, cameraAngle, 0);
        transform.rotation = relativeRotation;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        _camera = null;
    }
}
