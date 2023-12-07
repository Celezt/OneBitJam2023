using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoRenderer : MonoBehaviour
{
    [SerializeField]
    private Color _color = new Color(0, 0, 1, 0.8f);
    [SerializeField]
    private GizmoType _type = GizmoType.Sphere;
    [SerializeField]
    private Vector3 _offset = Vector3.zero;
    [SerializeField, ShowIf("@_type == GizmoType.Cube || _type == GizmoType.WireCube")]
    private Vector3 _scale = Vector3.one;
    [SerializeField, ShowIf("@_type == GizmoType.Sphere || _type == GizmoType.WireSphere")]
    private float _radius = 1.0f;
    [SerializeField, ShowIf("@_type == GizmoType.Cube || _type == GizmoType.WireCube")]
    private bool _useTransformScale;

    private enum GizmoType
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube,
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = _color;
        
        switch (_type)
        {
            case GizmoType.Sphere:
                Gizmos.DrawSphere(transform.position + _offset, _radius);
                break;
            case GizmoType.WireSphere:
                Gizmos.DrawWireSphere(transform.position + _offset, _radius);
                break;
            case GizmoType.Cube:
                Gizmos.DrawCube(transform.position + _offset, _useTransformScale ? Vector3.Scale(transform.localScale, _scale) : _scale);
                break;
            case GizmoType.WireCube:
                Gizmos.DrawWireCube(transform.position + _offset, _useTransformScale ? Vector3.Scale(transform.localScale, _scale) : _scale);
                break;
        }

        Gizmos.color = oldColor;
    }
}
