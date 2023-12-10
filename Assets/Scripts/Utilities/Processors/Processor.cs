using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Processor", menuName = "Processor")]
[HideMonoScript]
public class Processor : SerializedScriptableObject
{
    public IProcessor Value => _value;

    [SerializeField, InlineProperty, HideLabel]
    private IProcessor _value;
}
