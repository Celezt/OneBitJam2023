using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public Rigidbody Rigidbody { get; }

    public void Destroy();
}
