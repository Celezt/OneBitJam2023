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
    public Playlist Playlist => _playlist;

    [SerializeField]
    private AudioSource _audioSource;

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
    private Playlist _playlist;

    [SerializeField]
    private bool _scaleOfDelta = true;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfDelta))]
    private float _maxDelta = 8.0f;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfDelta)), MaxValue("@_maxDelta"), MinValue(0)]
    private float _minDelta = 1.0f;
    [SerializeField]
    private bool _hasExitTime = false;
    [SerializeField, ShowIf(nameof(_hasExitTime)), Indent]
    private float _exitTime = 0.2f;

    private Vector3 _position;
    private Vector3 _previousPosition;
    private Vector3 _scale;
    private Vector3 _previousScale;
    private Quaternion _rotation;
    private Quaternion _previousRotation;

    private CancellationTokenSource _cancellationTokenSource;

    public enum TransformType
    {
        Position,
        Rotation,
        Scale,
    }

    private void OnEnable()
    {
        CTSUtility.Reset(ref _cancellationTokenSource);
        CheckOnChangeAsync(_cancellationTokenSource.Token).Forget();
    }   

    private void OnDisable()
    {
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
                    if (_scaleOfDelta)
                    {
                        float interval = Mathf.Clamp01(delta / _maxDelta);
                        _audioSource.PlayOneShot(_playlist, delta > _minDelta ? interval : 0);
                    }
                    else
                        _audioSource.PlayOneShot(_playlist);

                    if (!_hasExitTime)
                        await UniTask.WaitWhile(() => _audioSource.isPlaying);
                    else
                        await UniTask.WaitForSeconds(_exitTime, cancellationToken: ct);

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
}

