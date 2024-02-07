using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider)), HideMonoScript]
public class TriggerHandler : MonoBehaviour
{
    public bool IsTriggered => _isTriggered;

    public event Action<Collider> OnTriggerEnterCallback = delegate { };
    public event Action<Collider> OnTriggerExitCallback = delegate { };

    [SerializeField]
    private string _tag;

    [SerializeField, Space(8)]
    private UnityEvent<Collider> _onTriggerEnterEvent;
    [SerializeField]
    private UnityEvent<Collider> _onTriggerExitEvent;

    private bool _isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(_tag) && _tag != other.tag)
            return;

        _isTriggered = true;
        OnTriggerEnterCallback(other);
        _onTriggerEnterEvent.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!string.IsNullOrEmpty(_tag) && _tag != other.tag)
            return;

        _isTriggered = false;
        OnTriggerExitCallback(other);
        _onTriggerExitEvent.Invoke(other);
    }
}
