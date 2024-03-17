using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class AreaOfEffect : MonoBehaviour
{
    private readonly static HashSet<IEffector> _effectors = new HashSet<IEffector>();
    private readonly static Collider[] _collider = new Collider[1024];
    private static int _count;

    [SerializeField]
    private Vector3 _offset;

    [SerializeField]
    private LayerMask _layerMask = ~0;
    [SerializeField]
    private Tag _tag;

    [SerializeField]
    private ColliderShape _shape = ColliderShape.Sphere;
    [SerializeField, Indent, ShowIf(nameof(_shape), ColliderShape.Sphere)]
    private float _radius = 4;
    [SerializeField, Indent, ShowIf(nameof(_shape), ColliderShape.Box)]
    private Vector3 _size = Vector3.one * 4;

    [SerializeField, Space(8), Unit(Units.Second)]
    private float _delay;

    [SerializeField, Space(8)]
    private List<Collider> _ignoreColliders;

    [SerializeReference, Space(8)]
    private List<IEffect> _effects;

    public enum ColliderShape
    {
        Sphere,
        Box
    }

    public void Invoke()
    {
        if (_delay > 0)
        {
            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(_delay, cancellationToken: destroyCancellationToken);
                OnAddEffects();
            });
        }
        else
            OnAddEffects();

        void OnAddEffects()
        {
            Vector3 worldOffset = transform.TransformDirection(_offset);
            Vector3 position = transform.position;

            switch (_shape)
            {
                case ColliderShape.Sphere:
                    _count = Physics.OverlapSphereNonAlloc(position + worldOffset, _radius, _collider, _layerMask);
                    break;
                case ColliderShape.Box:
                    _count = Physics.OverlapBoxNonAlloc(position + worldOffset, _size, _collider, Quaternion.identity, _layerMask);
                    break;
            }

            for (int i = 0; i < _count; i++)
            {
                Collider collider = _collider[i];

                if (!_tag.Filter(collider))
                    continue;

                if (_ignoreColliders.Contains(collider))
                    continue;

                if (_collider[i].TryGetComponentInParent(out IEffector effector)) // If effector exist on the object.
                {
                    if (_effectors.Contains(effector))
                        continue;

                    effector.AddEffects(_effects, gameObject);
                    _effectors.Add(effector);
                }
            }

            _effectors.Clear();
        }
    }

#if UNITY_EDITOR
    [SerializeField, HideInInspector]
    private bool _showCollider = true;

    private void OnDrawGizmos()
    {
        if (!_showCollider)
            return;

        Vector3 worldOffset = transform.TransformDirection(_offset);
        Vector3 position = transform.position;

        Color originalColor = Gizmos.color;
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        switch (_shape)
        {
            case ColliderShape.Sphere:
                Gizmos.DrawWireSphere(position + worldOffset, _radius);
                break;
            case ColliderShape.Box:
                Gizmos.DrawWireCube(position + worldOffset, _size);
                break;
        }

        Gizmos.color = originalColor;
    }

    [Button("@_showCollider ? \"Hide Collider\" : \"Show Collider\""), PropertyOrder(-1), PropertySpace(SpaceAfter = 8)]
    private void ShowGizmo()
        => _showCollider = !_showCollider;
#endif
}
