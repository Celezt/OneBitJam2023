using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerHandler : MonoBehaviour
{
    public bool IsTriggered => _isTriggered;

    public event Action<Collider> OnTriggerEnterCallback = delegate { };
    public event Action<Collider> OnTriggerExitCallback = delegate { };

    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;

    private bool _isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        _isTriggered = true;
        OnTriggerEnterCallback(other);
        OnTriggerEnterEvent.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _isTriggered = false;
        OnTriggerExitCallback(other);
        OnTriggerExitEvent.Invoke(other);
    }
}
