using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetonator
{
    /// <summary>
    /// Instance or activate a new bullet.
    /// </summary>
    public void Trigger();
}
