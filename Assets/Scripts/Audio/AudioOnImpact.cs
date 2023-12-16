using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

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
    public float Cooldown
    {
        get => _cooldown;
        set => _cooldown = value;
    }
    public float Distance
    {
        get => _distance;
        set => _distance = value;
    }
    public bool ScaleOfSpeed
    {
        get => _scaleOfSpeed;
        set
        {
            if (_scaleOfSpeed && _scaleOfSpeed != value)
                _audioSource.volume = _startVolume;

            _scaleOfSpeed = value;
        }
    }
    public LayerMask LayerMask
    {
        get => _layerMask;
        set => _layerMask = value;
    }

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private Transform _target;
    [SerializeField, Indent]
    private Quaternion _rotation;
    [SerializeField, Space(8)]
    private Audio _audio; 
    [SerializeField, Space(8)]
#if UNITY_EDITOR
    [OnValueChanged(nameof(ScaleOfSpeedChange))]
#endif
    private bool _scaleOfSpeed = true;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 8.0f;
    [SerializeField]
    private float _cooldown = 0.2f;
    [SerializeField]
    private float _distance = 0.1f;
    [SerializeField]
    private LayerMask _layerMask;

    private bool _isColliding;
    private Vector3 _previousPosition;
    private float _startVolume;
    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _previousPosition = transform.position;
    }

    private void OnEnable()
    {
        _startVolume = _audioSource.volume;

        CTSUtility.Reset(ref _cancellationTokenSource);
        CheckOnImpactAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        _audioSource.volume = _startVolume;
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
                    if (ScaleOfSpeed)
                    {
                        float speed = Vector3.Distance(position, _previousPosition) / Time.deltaTime;
                        float interval = Mathf.Clamp01(speed / _maxSpeed);
                        _audioSource.volume = interval * _startVolume;
                    }

                    _audioSource.Play(_audio);
                    _isColliding = true;

                    await UniTask.WaitForSeconds(_cooldown, cancellationToken: ct);
                }
            }
            else
                _isColliding = false;

            _previousPosition = position;

            await UniTask.WaitForFixedUpdate();
        }
    }

#if UNITY_EDITOR
    private void ScaleOfSpeedChange()
    {
        if (!_scaleOfSpeed && Application.isPlaying)
            _audioSource.volume = _startVolume;
    }
#endif
}
