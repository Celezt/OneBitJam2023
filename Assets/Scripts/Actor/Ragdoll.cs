using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class Ragdoll : MonoBehaviour
{
    public bool IsRagdoll => _isRagdoll;

    [SerializeField]
    private bool _activateOnStart = false;
    [SerializeField]
    private Animator _animator;

    [SerializeField, Space(8), DrawWithUnity]
    private Rigidbody[] _ragdollRigidbodies;
    [SerializeField, DrawWithUnity]
    private Rigidbody[] _actorRigidbodies;

    [SerializeField, Space(8)]
    private UnityEvent _onRagdollActivateEvent;
    [SerializeField]
    private UnityEvent _onRagdollDeactivateEvent;

    private List<Joint> _joints = new();
    private bool _isRagdoll;

    public void OnEnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
            rigidbody.velocity = Vector3.zero;
        }

        foreach (var joint in _joints)
        {
            joint.enableCollision = true;
        }

        foreach (var rigidbody in _actorRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        if (_animator)
            _animator.enabled = false;

        _isRagdoll = true;

        _onRagdollActivateEvent.Invoke();
    }

    public void OnDisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
        }

        foreach (var joint in _joints)
        {
            joint.enableCollision = false;
        }

        foreach (var rigidbody in _actorRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        if (_animator)
            _animator.enabled = true;

        _isRagdoll = false;

        _onRagdollDeactivateEvent.Invoke();
    }

    private void Awake()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            var joint = rigidbody.GetComponent<Joint>();
            if (joint != null)
                _joints.Add(joint);
        }
    }

    private void Start()
    {
        if (_activateOnStart)
            OnEnableRagdoll();
        else
            OnDisableRagdoll();
    }

#if UNITY_EDITOR
    private const string ACTIVATE_RAGDOLL = "Activate Ragdoll";
    private const string DEACTIVATE_RAGDOLL = "Deactivate Ragdoll";

    [Button("@_isRagdoll ? DEACTIVATE_RAGDOLL : ACTIVATE_RAGDOLL"), PropertyOrder(-1), HideInEditorMode]
    private void SwitchState()
    {
        if (!_isRagdoll)
            OnEnableRagdoll();
        else
            OnDisableRagdoll();
    }
#endif
}
