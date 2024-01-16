using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IHitAsync : IHit
{
    public UniTaskVoid UpdateAsync(CancellationToken cancellationToken, IEntity entity);
}
