using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public abstract class Condition : MonoBehaviour
{
    public abstract bool OnCondition();
}
