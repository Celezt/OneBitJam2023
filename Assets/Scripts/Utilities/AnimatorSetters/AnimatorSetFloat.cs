using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetFloat : AnimatorSetBase<float>
{
    [SerializeField]
    private string _id;

    protected override void Set(float value)
        => Animator.SetFloat(_id, value);
}
