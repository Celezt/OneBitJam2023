using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ITrajectoryAsync : ITrajectoryBase
{
    public UniTask UpdateAsync(Rigidbody rigidbody, CancellationToken cancellationToken);
}
