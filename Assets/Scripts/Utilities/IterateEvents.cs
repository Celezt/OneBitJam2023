using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class IterateEvents : MonoBehaviour
{
    [SerializeField]
    private bool _invokeOnStart;

    [SerializeField, Space(8)]
    private UnityEvent[] _events;

    private int _currentIndex;

    public void Next()
    {
        var unityEvent = _events[_currentIndex];

        if (unityEvent != null)
            unityEvent.Invoke();

        _currentIndex = (_currentIndex + 1) % _events.Length;
    }

    private void Start()
    {
        if (_invokeOnStart)
            Next();
    }
}
