using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

[ExecuteAlways, HideMonoScript]
public abstract class AnimatorSetBase<T> : MonoBehaviour where T : new()
{
    public Animator Animator => _animator;

    private static IEnumerable<Type> ProcessorFilter => _cachedFilter ??= ReflectionUtility.GetDerivedTypes<IProcessor<T>>(AppDomain.CurrentDomain);

    private static IEnumerable<Type> _cachedFilter;

    [SerializeField, HideIf(nameof(_containsAnimator))]
    private Animator _animator;

    [SerializeReference, PropertyOrder(int.MaxValue), Space(8), TypeFilter(nameof(ProcessorFilter))]
    private List<IProcessor> _processors = new();

    private bool _containsAnimator;

    public void SetParameter(T value)
    {
        foreach (var processor in _processors)
        {
            if (processor is IProcessor<T> p)
                value = p.Process(value);
        }

        Set(value);
    }

    protected abstract void Set(T value);

    private void Start()
    {
        var animator = GetComponent<Animator>();

        if (animator != null)
        {
            _animator = animator;
            _containsAnimator = true;
        }
    }
}
