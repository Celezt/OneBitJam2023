using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : AnimatorSetBase<bool>
{
    [SerializeField]
    private string _id;

    protected override void Set(bool value)
        => Animator.SetBool(_id, value);
}
