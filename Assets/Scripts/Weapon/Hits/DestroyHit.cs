using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHit : IHit
{
    public void Initialize(IEntity entity)
    {
        entity.Destroy();
    }
}
