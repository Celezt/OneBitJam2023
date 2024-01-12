using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BurstDetonation : IDetonationAsync
{
    public float Cooldown => _cooldown + Duration * Amount;

    [MinValue(0)]
    public int Amount = 3;
    [MinValue(0)]
    public float Duration = 0.1f;

    [SerializeField, MinValue(0)]
    private float _cooldown = 0.5f;

    public async UniTask UpdateAsync(IDetonator handler, CancellationToken cancellationToken)
    {
        for (int i = 0; i < Amount; i++)
        {
            handler.Trigger();

            await UniTask.WaitForSeconds(Duration);
        }
    }
}
