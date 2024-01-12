using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetonation : IDetonationBase
{
    public void Initialize(IDetonator handler);
}
