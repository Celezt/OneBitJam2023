using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private static Camera _camera;

    [SerializeField]
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

        Vector3 forward = ActiveFacedObject.forward.x_z().normalized;
        transform.rotation = _initialRotation * Quaternion.Euler(forward);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        _camera = null;
    }
}
