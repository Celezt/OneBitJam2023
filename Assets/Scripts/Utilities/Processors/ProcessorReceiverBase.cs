using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[HideMonoScript]
public abstract class ProcessorReceiverBase<T> : MonoBehaviour where T : struct
{
    private static IEnumerable<Type> ProcessorFilter => _cachedFilter ??= ReflectionUtility.GetDerivedTypes<IProcessor<T>>(AppDomain.CurrentDomain);

    private static IEnumerable<Type> _cachedFilter;

    public IReadOnlyList<IProcessor> Processors => _processors;
    public UnityEvent<T> OnInvokeCallback => _onInvokeCallback;
    public InvokeTypes Types => _invokeTypes;
    public T Value
    {
        get => _value;
        set => _value = value;
    }
    public T ProcessedValue => IProcessor.Process(_processors, _value);

    [SerializeField, PropertySpace(SpaceAfter = 8)]
    private InvokeTypes _invokeTypes = InvokeTypes.Update;

    [SerializeReference, PropertyOrder(int.MaxValue), Space(8), TypeFilter(nameof(ProcessorFilter))]
    private List<IProcessor> _processors = new();

    [SerializeField]
    private UnityEvent<T> _onInvokeCallback;

    private T _value;

    [Flags]
    public enum InvokeTypes
    {
        Update          = 1 << 0,
        FixedUpdate     = 1 << 1,
        LateUpdate      = 1 << 2,
        Start           = 1 << 3,
        OnEnable        = 1 << 4,
        OnDisable       = 1 << 5,
        OnDestroy       = 1 << 6,
    }

    public void SetValue(InputAction.CallbackContext callback)
        => SetValue(callback.ReadValue<T>());
    public void SetValue(T value)
        => _value = value;

    private void Start()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.Start))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void OnEnable()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.OnEnable))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void OnDisable()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.OnDisable))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void OnDestroy()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.OnDestroy))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void Update()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.Update))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void LateUpdate()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.LateUpdate))
            _onInvokeCallback.Invoke(ProcessedValue);
    }

    private void FixedUpdate()
    {
        if (_invokeTypes.HasFlag(InvokeTypes.FixedUpdate))
            _onInvokeCallback.Invoke(ProcessedValue);
    }
}
