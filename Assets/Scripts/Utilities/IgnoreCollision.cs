using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), HideMonoScript]
public class IgnoreCollisions : MonoBehaviour
{
    [SerializeField]
    private Collider[] _ignoreColliders;

    private void Start()
    {
        var collider = GetComponent<Collider>();

        for (int i = 0; i < _ignoreColliders.Length; i++)
            Physics.IgnoreCollision(collider, _ignoreColliders[i]);
    }
}
