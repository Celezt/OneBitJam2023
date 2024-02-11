using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class RagdollDetach : MonoBehaviour, IRagdoll
{
    public bool IsRagdoll => _isRagdoll;
    public bool TeleportOnDeactivate
    {
        get => _teleportOnDeactivate;
        set => _teleportOnDeactivate = value;

    }

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private bool _activateOnStart = false;

    [SerializeField, Space(4)]
    private bool _resetTransformOnDeactivate = true;
    [SerializeField]
    private bool _teleportOnDeactivate = false;
    [SerializeField, ShowIf(nameof(_teleportOnDeactivate)), Indent]
    private Transform _copyTransformOnTeleport;

    [SerializeField, PropertySpace(SpaceBefore = 4)]
    private Rigidbody _actorRigidbody;

    [SerializeField, Space(8), DisableInPlayMode]
    private SkinnedMeshRenderer[] _shinnedMeshes;
    [SerializeField, TableList, PropertySpace(8, 4), DisableInPlayMode]
    private List<RagdollData> _ragdollRigidbodies;

    [FoldoutGroup("Events"), SerializeField]
    private UnityEvent _onRagdollActivateEvent;
    [FoldoutGroup("Events"), SerializeField]
    private UnityEvent _onRagdollDeactivateEvent;

    private bool _isRagdoll;
    private bool _isInitialized;
    private CachedPush? _cachedPush;
    private Vector3[] _initialPositions;
    private Quaternion[] _initialRotation;

    [Serializable]
    private struct RagdollData
    {
        public Rigidbody Rigidbody;
        [TableColumnWidth(50, Resizable = false), VerticalGroup("Hide"), HideLabel]
        public bool HideMesh;
    }

    private struct CachedPush
    {
        public float Force;
        public Vector3 Position;
        public float Radius;
        public float UpwardModifier;
        public float TimeSnapshot;
    }

    public void Push(float force, Vector3 position, float radius, float upwardsModifier = 1.0f)
    {
        if (!_isRagdoll)    // Cash push if currently not in ragdoll form.
        {
            _cachedPush = new CachedPush
            {
                Force = force,
                Position = position,
                Radius = radius,
                UpwardModifier = upwardsModifier,
                TimeSnapshot = Time.time
            };
        }
        else
        {
            for (int i = 0; i < _ragdollRigidbodies.Count; i++)
            {
                var rigidbody = _ragdollRigidbodies[i];
                rigidbody.Rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

    public void OnEnableRagdoll()
    {
        if (_isRagdoll && _isInitialized)
            return;

        _isRagdoll = true;

        foreach (var skinnedMeshRenderer in _shinnedMeshes)
        {
            skinnedMeshRenderer.enabled = false;
        }

        foreach (var data in _ragdollRigidbodies)
        {
            data.Rigidbody.detectCollisions = true;
            data.Rigidbody.isKinematic = false;

            if (data.HideMesh && data.Rigidbody.TryGetComponent<Renderer>(out var renderer))
                renderer.enabled = true;
        }

        if (_actorRigidbody)
        {
            _actorRigidbody.detectCollisions = false;
            _actorRigidbody.isKinematic = true;
        }

        if (_animator)
            _animator.enabled = false;

        if (_cachedPush is { } cached)
        {
            if (Time.time - cached.TimeSnapshot > 1f)   // Remove cached if it is older than 1 sec.
                _cachedPush = null;
            else
                Push(cached.Force, cached.Position, cached.Radius, cached.UpwardModifier);
        }

        _onRagdollActivateEvent.Invoke();
    }

    public void OnDisableRagdoll()
    {
        if (!_isRagdoll && _isInitialized)
            return;

        _isRagdoll = false;

        if (_actorRigidbody)
        {
            if (_isInitialized && _teleportOnDeactivate && _copyTransformOnTeleport)
            {
                _actorRigidbody.position = _copyTransformOnTeleport.position;
                _actorRigidbody.rotation.SetLookRotation(_copyTransformOnTeleport.forward._y_().normalized);
            }

            _actorRigidbody.detectCollisions = true;
            _actorRigidbody.isKinematic = false;
        }

        foreach (var skinnedMeshRenderer in _shinnedMeshes)
            skinnedMeshRenderer.enabled = true;

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            var data = _ragdollRigidbodies[i];
            if (_resetTransformOnDeactivate)
            {
                var ragdollTransform = data.Rigidbody.transform;
                ragdollTransform.localPosition = _initialPositions[i];
                ragdollTransform.localRotation = _initialRotation[i];
            }

            data.Rigidbody.detectCollisions = false;
            data.Rigidbody.isKinematic = true;

            if (data.HideMesh && data.Rigidbody.TryGetComponent<Renderer>(out var renderer))
                renderer.enabled = false;
        }

        if (_animator)
            _animator.enabled = true;

        _onRagdollDeactivateEvent.Invoke();
    }

    private void Awake()
    {
        int length = _ragdollRigidbodies.Count;
        _initialPositions = new Vector3[length];
        _initialRotation = new Quaternion[length];

        for (int i = 0; i < length; i++)
        {
            var data = _ragdollRigidbodies[i];
            var ragdollTransform = data.Rigidbody.transform;

            _initialPositions[i] = ragdollTransform.localPosition;
            _initialRotation[i] = ragdollTransform.localRotation;
        }
    }

    private void Start()
    {
        if (_activateOnStart)
            OnEnableRagdoll();
        else
            OnDisableRagdoll();

        _isInitialized = true;
    }

#if UNITY_EDITOR
    [Button("@_isRagdoll ? \"Deactivate Ragdoll\" : \"Activate Ragdoll\""), PropertyOrder(-2), HideInEditorMode]
    private void SwitchState()
    {
        if (!_isRagdoll)
            OnEnableRagdoll();
        else
            OnDisableRagdoll();
    }

    [Button("Hide Meshes"), PropertyOrder(-1), HideInPlayMode]
    private void HideMeshes()
    {
        foreach (var data in _ragdollRigidbodies)
        {
            if (data.HideMesh && data.Rigidbody.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.enabled = false;
            }
        }
    }
#endif
}
