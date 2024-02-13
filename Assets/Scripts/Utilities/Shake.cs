using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[HideMonoScript]
public class Shake : MonoBehaviour
{
    [SerializeField, Space(4)]
    private Vector3 _velocity = Vector3.one * 10;
    [SerializeField]
    private Vector3 _maxOffset = Vector3.one * 8;
    [SerializeField]
    private Vector3 _timeOffset = Vector3.zero;

    [SerializeField, Space(4)]
    private bool _shakeOnEnable = false;
    [SerializeField]
    private bool _hasDuration = true;
    [SerializeField, ShowIf(nameof(_hasDuration)), Indent]
    private float _duration = 4;
    [SerializeField, ShowIf(nameof(_hasDuration)), Indent, PropertyRange(0, "@_duration")]
    private float _blendDuration = 1;

    private float _startTime;
    private CancellationTokenSource _cancellationTokenSource;

    public void OnShake()
    {
        _startTime = Time.time;
        if (_cancellationTokenSource == null)
        {
            CTSUtility.Reset(ref _cancellationTokenSource);
            OnShakeAsync(_cancellationTokenSource.Token)
                .ContinueWith(() => CTSUtility.Clear(ref _cancellationTokenSource));
        }
    }

    private void OnEnable()
    {
        if (_shakeOnEnable)
            OnShake();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private async UniTask OnShakeAsync(CancellationToken cancellationToken)
    {
        Vector3 startPosition = transform.position;

        while(!cancellationToken.IsCancellationRequested)
        {
            float time = Time.time;

            if (_hasDuration && time - _startTime > _duration)
                break;

            Vector3 offset = new Vector3
                (
                    Mathf.Sin((time + _timeOffset.x) * _velocity.x) * _maxOffset.x,
                    Mathf.Sin((time + _timeOffset.y) * _velocity.y) * _maxOffset.y,
                    Mathf.Sin((time + _timeOffset.z) * _velocity.z) * _maxOffset.z
                );

            if (_hasDuration && _blendDuration > 0 && time - _startTime > _duration - _blendDuration)
            {
                float interval = (time - _startTime - _duration + _blendDuration) / _blendDuration;
                offset *= 1 - interval;

                transform.position = Vector3.Lerp(transform.position, startPosition, interval) + offset;
            }
            else
                transform.position = startPosition + offset;

            await UniTask.Yield(destroyCancellationToken);
        }

        transform.position = startPosition;
    }
}
