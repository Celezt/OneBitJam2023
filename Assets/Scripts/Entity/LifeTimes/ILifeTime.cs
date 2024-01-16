using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ILifeTime
{
    public UniTask DurationAsync(CancellationToken cancellationToken, IEntity entity);
}
