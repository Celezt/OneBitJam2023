using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class Hitbox : MonoBehaviour, IHitbox
{
    public GameObject Parent => _parent;

    [SerializeField]
    private GameObject _parent;
}
