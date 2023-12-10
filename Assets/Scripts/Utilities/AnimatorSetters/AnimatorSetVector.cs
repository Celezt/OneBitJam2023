using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetVector : AnimatorSetBase<Vector2>
{
    [SerializeField]
    private string _IdX;
    [SerializeField]
    private string _IdY;

    protected override void Set(Vector2 value)
    {
        Animator.SetFloat(_IdX, value.x);
        Animator.SetFloat(_IdY, value.y);
    }
}
