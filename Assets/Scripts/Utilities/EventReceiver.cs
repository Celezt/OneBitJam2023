using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class EventReceiver : MonoBehaviour
{
    public UnityEvent OnInvokeEvent;

    public void Invoke() 
        => OnInvokeEvent.Invoke();
}
