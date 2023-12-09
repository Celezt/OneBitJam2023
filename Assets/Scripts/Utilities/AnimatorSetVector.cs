using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

[ExecuteAlways, HideMonoScript]
public class AnimatorSetVector : MonoBehaviour
{
    [SerializeField, HideIf(nameof(_containsAnimator))]
    private Animator _animator;
    [SerializeField]
    private string _IdX;
    [SerializeField]
    private string _IdY;

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

    public void SetParameter(Vector2 value)
    {
        _animator.SetFloat(_IdX, value.x);
        _animator.SetFloat(_IdY, value.y);
    }
}
