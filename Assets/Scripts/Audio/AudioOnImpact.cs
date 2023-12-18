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
    public AudioSource CurrentSource => _audioSources.Count > _audioSourceIndex ? _audioSources[_audioSourceIndex] : null;
    public Playlist Playlist => _playlist;

    [SerializeField]
    private Transform _target;
    [SerializeField, Indent]
    private Quaternion _rotation;
    [SerializeField]
    private float _distance = 0.1f;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private List<AudioSource> _audioSources;

    [SerializeField]
    private Playlist _playlist; 
    [SerializeField]
#if UNITY_EDITOR
    [OnValueChanged(nameof(ScaleOfSpeedChange))]
#endif
    private bool _scaleOfSpeed = true;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 8.0f;
    [SerializeField]
    private bool _hasExitTime = false;
    [SerializeField, ShowIf(nameof(_hasExitTime)), Indent]
    private float _exitTime = 0.2f;

    private bool _isColliding;
    private int _audioSourceIndex;
    private Vector3 _previousPosition;

    private Dictionary<AudioSource, float> _startVolumes;
    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _previousPosition = transform.position;
    }

    private void OnEnable()
    {
        _startVolumes = DictionaryPool<AudioSource, float>.Get();

        foreach (var source in _audioSources)
            _startVolumes[source] = source.volume;

        CTSUtility.Reset(ref _cancellationTokenSource);
        CheckOnImpactAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        foreach (var source in _audioSources)
        {
            if (_startVolumes.TryGetValue(source, out var volume))
                source.volume = volume;
        }

        DictionaryPool<AudioSource, float>.Release(_startVolumes);

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
                var source = CurrentSource;

                if (!_isColliding && source)
                {
                    if (_scaleOfSpeed && _startVolumes.TryGetValue(source, out float volume))
                    {
                        float speed = Vector3.Distance(position, _previousPosition) / Time.deltaTime;
                        float interval = Mathf.Clamp01(speed / _maxSpeed);
                        source.volume = interval * volume;
                    }

                    source.Play(_playlist);
                    _isColliding = true;

                    _audioSourceIndex++;
                    _audioSourceIndex %= _audioSources.Count;

                    if (!_hasExitTime)
                        await UniTask.WaitWhile(() => source.isPlaying);
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

#if UNITY_EDITOR
    private void ScaleOfSpeedChange()
    {
        if (!_scaleOfSpeed && Application.isPlaying)
        {
            foreach (var source in _audioSources)
            {
                if (_startVolumes.TryGetValue(source, out var volume))
                    source.volume = volume;
            }
        }
    }
#endif
}
