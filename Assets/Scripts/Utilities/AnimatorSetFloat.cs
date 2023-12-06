using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

[ExecuteAlways, HideMonoScript]
public class AnimatorSetFloat : MonoBehaviour
{
    [SerializeField, HideIf(nameof(_containsAnimator))]
    private Animator _animator;
    [SerializeField]
    private string _id;

    private bool _containsAnimator;

    private void Start()
    {
        var animator = GetComponent<Animator>();

        if (animator != null)
        {
            _animator = animator;
            _containsAnimator = true;
        }
    }

    public void SetParameter(float value)
        => _animator.SetFloat(_id, value);
}
