using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    private static readonly int MOVE_SPEED_ID = Animator.StringToHash("MoveSpeed");

    [SerializeField]
    private Animator _animator;

    public void SetSpeed(float speed)
        => _animator.SetFloat(MOVE_SPEED_ID, speed);
}
