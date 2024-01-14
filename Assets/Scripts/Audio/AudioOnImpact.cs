using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

[HideMonoScript]
public class AudioOnImpact : MonoBehaviour
{
    public Quaternion Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }
    public float MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }
    public float ExitTime
    {
        get => _exitTime;
        set => _exitTime = value;
    }
    public float Distance
    {
        get => _distance;
        set => _distance = value;
    }
    public LayerMask LayerMask
    {
        get => _layerMask;
        set => _layerMask = value;
    }
    public Playlist Playlist => _playlist;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Transform _target;
    [SerializeField, Indent]
    private Quaternion _rotation;
    [SerializeField]
    private float _distance = 0.1f;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private Playlist _playlist; 
    [SerializeField]
    private bool _scaleOfSpeed = true;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 6.0f;
    [SerializeField]
    private bool _hasExitTime = false;
    [SerializeField, ShowIf(nameof(_hasExitTime)), Indent]
    private float _exitTime = 0.2f;

    private bool _isColliding;
    private Vector3 _previousPosition;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _previousPosition = transform.position;
    }

    private void OnEnable()
    {
        CTSUtility.Reset(ref _cancellationTokenSource);
        CheckOnImpactAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private void OnDrawGizmos()
    {
        if (_target == null)
            return;

        Color oldColor = Gizmos.color;
        Gizmos.color = Color.yellow;
        Vector3 position = _target.position;
        Gizmos.DrawRay(position, _rotation * Vector3.down * _distance);
        Gizmos.color = oldColor;
    }

    private async UniTask CheckOnImpactAsync(CancellationToken ct)
    {
        while(!ct.IsCancellationRequested)
        {
            Vector3 position = _target.position;

            if (Physics.Raycast(position, _rotation * Vector3.down, out RaycastHit hit, _distance, _layerMask))
            {
                if (!_isColliding)
                {
                    if (_scaleOfSpeed)
                    {
                        float speed = Vector3.Distance(position, _previousPosition) / Time.deltaTime;
                        float interval = Mathf.Clamp01(speed / _maxSpeed);

                        _audioSource.PlayOneShot(_playlist, interval);
                    }
                    else
                        _audioSource.PlayOneShot(_playlist);

                    _isColliding = true;

                    if (!_hasExitTime)
                        await UniTask.WaitWhile(() => _audioSource.isPlaying);
                    else
                        await UniTask.WaitForSeconds(_exitTime, cancellationToken: ct);
                }
            }
            else
                _isColliding = false;

            _previousPosition = position;

            await UniTask.WaitForFixedUpdate();
        }
    }
}
