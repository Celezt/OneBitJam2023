using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[HideMonoScript]
public class AudioOnSliding : MonoBehaviour
{
    [SerializeField]
    protected AudioSource _audioSource;
    [SerializeField]
    protected Rigidbody _target;
    [SerializeField]
    protected LayerMask _layerMask = ~0;

#if UNITY_EDITOR
    [OnValueChanged(nameof(OnScaleOfSpeedChange))]
#endif
    [SerializeField]
    private bool _scaleOfSpeed = false;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 2.0f;
    [SerializeField, Indent]
    private float _minSpeedStart = 0.25f;
    [SerializeField, Indent]
    private float _minSpeedEnd = 0.01f;

    [SerializeField, PropertyRange(0, 90)]
    private float _surfaceAngle = 90;

    [SerializeField]
    protected Playlist _playlist;

    private HashSet<GameObject> _surfaceGameObjects = new();
    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        if (_target == null)
            return;

        CTSUtility.Reset(ref _cancellationTokenSource);
        OnStayAsync(_cancellationTokenSource.Token).Forget();
        OnExitAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
        _surfaceGameObjects.Clear();
    }

    private UniTask OnStayAsync(CancellationToken cancellationToken)
        => _target.GetAsyncCollisionStayTrigger().ForEachAsync(collision =>
        {
            if (((1 << collision.gameObject.layer) & _layerMask) != 0)
            {
                if (Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.up)) >= (180f - _surfaceAngle) / 180f)
                    _surfaceGameObjects.Add(collision.gameObject);
                else
                    _surfaceGameObjects.Remove(collision.gameObject);

                if (_surfaceGameObjects.Count > 0)
                {
                    Vector3 velocity = _target.linearVelocity;
                    float perpendicularInterval = 1f - Mathf.Abs(Vector3.Dot(velocity.normalized, Physics.gravity.normalized));
                    float slideSpeed = velocity.magnitude * perpendicularInterval;
                    float speedInterval = Mathf.Clamp01(slideSpeed / _maxSpeed);

                    switch (_audioSource.isPlaying)
                    {
                        case true when slideSpeed < _minSpeedEnd:
                        case false when slideSpeed < _minSpeedStart:
                            _audioSource.Stop();
                            break;
                        case false when _scaleOfSpeed:
                            _audioSource.Play(_playlist, speedInterval);
                            break;
                        case false:
                            _audioSource.Play(_playlist);
                            break;
                        case true when _scaleOfSpeed:
                            _audioSource.SetVolumeScale(speedInterval);
                            break;
                    }
                }
                else
                    _audioSource.Stop();
            }
        }, cancellationToken);

    private UniTask OnExitAsync(CancellationToken cancellationToken)
        => _target.GetAsyncCollisionExitTrigger().ForEachAsync(collision =>
        {
            _surfaceGameObjects.Remove(collision.gameObject);

            if (_surfaceGameObjects.Count <= 0)
                _audioSource.Stop();
        }, cancellationToken);

#if UNITY_EDITOR
    private void OnScaleOfSpeedChange()
    {
        if (!_scaleOfSpeed && _audioSource)
            _audioSource.SetVolumeScale(1);
    }
#endif
}
