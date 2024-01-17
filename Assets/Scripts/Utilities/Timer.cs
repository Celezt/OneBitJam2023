using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class Timer : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onFinishEvent;

    private CancellationTokenSource _cancellationTokenSource;

    public void OnStartTimer(float time)
    {
        CTSUtility.Reset(ref _cancellationTokenSource);
        TimerAsync(_cancellationTokenSource.Token, time).Forget();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private async UniTaskVoid TimerAsync(CancellationToken cancellationToken, float time)
    {
        await UniTask.WaitForSeconds(time, cancellationToken: cancellationToken);

        _onFinishEvent.Invoke();
    }
}
