using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoRenderer : MonoBehaviour
{
    [SerializeField]
    private Color _color = new Color(0, 0, 1, 0.8f);
    [SerializeField]
    private GizmoTypes _type = GizmoTypes.Sphere;
    [SerializeField]
    private Vector3 _offset = Vector3.zero;
    [SerializeField, ShowIf("@_type == GizmoTypes.Cube || _type == GizmoTypes.WireCube")]
    private Vector3 _scale = Vector3.one;
    [SerializeField, ShowIf("@_type == GizmoTypes.Sphere || _type == GizmoTypes.WireSphere")]
    private float _radius = 1.0f;
    [SerializeField, ShowIf("@_type == GizmoTypes.Cube || _type == GizmoTypes.WireCube")]
    private bool _useTransformScale;
    [SerializeField, ShowIf("@_type == GizmoTypes.Line")]
    private Quaternion _rotation = Quaternion.identity;
    [SerializeField, ShowIf("@_type == GizmoTypes.Line")]
    private float _length = 1.0f;

    private enum GizmoTypes
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube,
        Line,
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = _color;

        Vector3 position = transform.position;

        switch (_type)
        {
            case GizmoTypes.Sphere:
                Gizmos.DrawSphere(position + _offset, _radius);
                break;
            case GizmoTypes.WireSphere:
                Gizmos.DrawWireSphere(position + _offset, _radius);
                break;
            case GizmoTypes.Cube:
                Gizmos.DrawCube(position + _offset, _useTransformScale ? Vector3.Scale(transform.localScale, _scale) : _scale);
                break;
            case GizmoTypes.WireCube:
                Gizmos.DrawWireCube(position + _offset, _useTransformScale ? Vector3.Scale(transform.localScale, _scale) : _scale);
                break;
            case GizmoTypes.Line:
                Vector3 worldOffset = transform.TransformDirection(_offset );
                Gizmos.DrawLine(position + worldOffset, position + worldOffset + (transform.rotation * _rotation) * Vector3.forward * _length);
                break;
        }

        Gizmos.color = oldColor;
    }
}
