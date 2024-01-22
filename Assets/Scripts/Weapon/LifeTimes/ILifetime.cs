using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ILifetime
{
    public UniTask UpdateAsync(CancellationToken cancellationToken, IEntity entity, Weapon weapon);
}
