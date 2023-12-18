using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

[HideMonoScript]
public class AudioOnTransform : MonoBehaviour
{
    public float ExitTime
    {
        get => _exitTime;
        set => _exitTime = value;
    }
    public AudioSource CurrentSource => _audioSources.Count > _audioSourceIndex ? _audioSources[_audioSourceIndex] : null;
    public Playlist Playlist => _playlist;

    [SerializeField, Required]
    private Transform _target;
    [SerializeField]
    private TransformType _type = TransformType.Position;
    [SerializeField, Indent, ShowIf(nameof(_type), TransformType.Scale)]
    private bool _isLocal = false;
    [SerializeField, LabelText("X"), HorizontalGroup(LabelWidth = 12, Title = "Constrained Axes", PaddingLeft = 20)]
    private bool _isConstraintX = true;
    [SerializeField, LabelText("Y"), HorizontalGroup(LabelWidth = 12)]
    private bool _isConstraintY = true;
    [SerializeField, LabelText("Z"), HorizontalGroup(LabelWidth = 12)]
    private bool _isConstraintZ = true;

    [SerializeField]
    private List<AudioSource> _audioSources;

    [SerializeField]
    private Playlist _playlist;

    [SerializeField]
#if UNITY_EDITOR
    [OnValueChanged(nameof(ScaleOfDelta))]
#endif
    private bool _scaleOfDelta = true;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfDelta))]
    private float _maxDelta = 8.0f;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfDelta)), MaxValue("@_maxDelta"), MinValue(0)]
    private float _minDelta = 1.0f;
    [SerializeField]
    private bool _hasExitTime = false;
    [SerializeField, ShowIf(nameof(_hasExitTime)), Indent]
    private float _exitTime = 0.2f;

    private int _audioSourceIndex;
    private Vector3 _position;
    private Vector3 _previousPosition;
    private Vector3 _scale;
    private Vector3 _previousScale;
    private Quaternion _rotation;
    private Quaternion _previousRotation;

    private Dictionary<AudioSource, float> _startVolumes;
    private CancellationTokenSource _cancellationTokenSource;

    public enum TransformType
    {
        Position,
        Rotation,
        Scale,
    }

    private void OnEnable()
    {
        _startVolumes = DictionaryPool<AudioSource, float>.Get();

        foreach (var source in _audioSources)
            _startVolumes[source] = source.volume;

        CTSUtility.Reset(ref _cancellationTokenSource);
        CheckOnChangeAsync(_cancellationTokenSource.Token).Forget();
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

    private void FixedUpdate()
    {
        _previousPosition = _position;
        _previousRotation = _rotation;
        _previousScale = _scale;

        _position = _target.position;
        _rotation = _target.rotation;
        _scale = _isLocal ? _target.localScale : _target.lossyScale;
    }

    private async UniTask CheckOnChangeAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            float deltaTime = Time.deltaTime;
            float delta = 0;

            switch (_type)
            {
                case TransformType.Position:
                    delta = Vector3.Distance(_position, _previousPosition) / deltaTime;

                    if (HasChanged(_position, _previousPosition))
                        goto default;

                    break;
                case TransformType.Rotation:
                    delta = Quaternion.Angle(_rotation, _previousRotation);

                    if (HasChanged(_rotation, _previousRotation))
                        goto default;

                    break;
                case TransformType.Scale:
                    delta = Vector3.Distance(_scale, _previousScale) / deltaTime;

                    if (HasChanged(_scale, _previousScale))
                        goto default;

                    break;
                default:
                    var source = CurrentSource;

                    if (source)
                    {
                        if (_scaleOfDelta && _startVolumes.TryGetValue(source, out float volume))
                        {
                            if (_minDelta > delta)
                                source.volume = 0;
                            else
                            {
                                float interval = Mathf.Clamp01(delta / _maxDelta);
                                source.volume = interval * volume;
                            }
                        }

                        source.Play(_playlist);

                        _audioSourceIndex++;
                        _audioSourceIndex %= _audioSources.Count;

                        if (!_hasExitTime)
                            await UniTask.WaitWhile(() => source.isPlaying);
                        else
                            await UniTask.WaitForSeconds(_exitTime, cancellationToken: ct);
                    }

                    break;
            }

            await UniTask.WaitForFixedUpdate();
        }
    }

    private bool HasChanged(Vector3 vector, Vector3 previousVector)
    {
        if (_isConstraintX && !AlmostEquals(vector.x, previousVector.x, 0.1f))
            return true;

        if (_isConstraintY && !AlmostEquals(vector.y, previousVector.y, 0.1f))
            return true;

        if (_isConstraintZ && !AlmostEquals(vector.z, previousVector.z, 0.1f))
            return true;

        return false;
    }

    private bool HasChanged(Quaternion quaternion, Quaternion previousQuaternion)
    {
        if (_isConstraintX && !AlmostEquals(quaternion.x, previousQuaternion.x, 0.005f))
            return true;

        if (_isConstraintY && !AlmostEquals(quaternion.y, previousQuaternion.y, 0.005f))
            return true;

        if (_isConstraintZ && !AlmostEquals(quaternion.z, previousQuaternion.z, 0.005f))
            return true;

        return false;
    }

    private static bool AlmostEquals(float valueFirst, float valueSecond, float precision)
        => Mathf.Abs(valueFirst - valueSecond) <= precision;

#if UNITY_EDITOR
    private void ScaleOfDelta()
    {
        if (!_scaleOfDelta && Application.isPlaying)
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

