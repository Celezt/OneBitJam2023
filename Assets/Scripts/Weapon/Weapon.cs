using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

[HideMonoScript]
public class Weapon : MonoBehaviour, IDetonator
{
    public GameObject BulletPrefab
    {
        get => _bulletPrefab;
        set
        {
            GameObject previousBullet = _bulletPrefab;
            _bulletPrefab = value;

            if (_bulletPrefab != previousBullet) // Invoke if not the same bullet prefab.
                OnBulletChangeCallback(_bulletPrefab);
        }
    }
    public WeaponHandler Handler
    {
        get => _handler;
        set
        {
            var newHandler = value;

            if (newHandler != _handler)
            {
                if (_bulletPool != null)
                    _bulletPool.Dispose();

                if (newHandler != null)
                    _bulletPool = CreateBulletPool(newHandler.IgnoreColliders, newHandler.TeamTag);
            }

            _handler = newHandler;
        }
    }
    public float Cooldown => _detonation.Cooldown;
    public bool IsAutomatic
    {
        get => _isAutomatic;
        set => _isAutomatic = value;
    }

    public event Action<GameObject> OnBulletChangeCallback = delegate { };

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField, AssetsOnly, Space(8)]
    private GameObject _bulletPrefab;
    [SerializeField, Indent, MinValue(0)]
    private int _maxBulletCapacity = 100;
    [SerializeField, Indent(2), MinValue(0), MaxValue("@_maxBulletCapacity")]
    private int _defaultBulletSize = 10;

    [SerializeField, Indent]
    private Vector3 _offset;
    [SerializeField, Indent]
    private Quaternion _rotation = Quaternion.identity;

    [SerializeReference, Space(8)]
    private IDetonation _detonation = new InstantDetonation();

    [SerializeReference, Space(8)]
    private ISpread _spread = new RandomSpread();

    [SerializeField, Space(8)]
    private bool _useMoveVelocity = true;
    [SerializeField, Indent, ShowIf(nameof(_useMoveVelocity))]
    private float _moveVelocityScale = 0.8f;
    [SerializeField]
    private bool _isAutomatic;

    [SerializeField, Space(8)]
    private Playlist _shootingPlaylist;

    private WeaponHandler _handler;
    private ObjectPool<Bullet> _bulletPool;
    private CancellationTokenSource _detonationCancellationTokenSource;

    public void Trigger()
    {
        if (_bulletPrefab == null)
            return;

        Vector3 position = transform.TransformDirection(_offset) + transform.position;
        Quaternion rotation = _spread.Rotation(transform.rotation * _rotation);

        Vector3 moveVelocity = Vector3.zero;
        if (_useMoveVelocity && _handler.MoveRigidbody)
            moveVelocity = _handler.MoveRigidbody.velocity * _moveVelocityScale;

        Bullet bullet = _bulletPool.Get();
        bullet.transform.position = position;
        bullet.Pool = _bulletPool;

        bullet.Shoot(position, rotation, moveVelocity);

        if (_audioSource != null)
            _audioSource.PlayOneShot(_shootingPlaylist);
    }

    public void Shoot()
    {
        _detonation.Initialize(this);

        if (_detonation is IDetonationAsync detonationAsync)
        {
            CTSUtility.Reset(ref _detonationCancellationTokenSource);
            detonationAsync.UpdateAsync(this, _detonationCancellationTokenSource.Token).Forget();
        }
    }

    private void Start()
    {
        if (_handler == null)
        {
            _handler = GetComponentInParent<WeaponHandler>();

            if (_handler != null)
                _bulletPool = CreateBulletPool(_handler.IgnoreColliders, _handler.TeamTag);
        }
    }

    private void OnDestroy()
    {
        if (_bulletPool != null)
            _bulletPool.Dispose();

        CTSUtility.Clear(ref _detonationCancellationTokenSource);
    }

    private Bullet CreateBullet(IReadOnlyList<Collider> ignoreColliders = null, string teamTag = null)
    {
        if (BulletPrefab == null)
            return null;

        GameObject bulletObject = Instantiate(_bulletPrefab);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.IgnoreCollisions(ignoreColliders);
        bullet.TeamTag = teamTag;

        return bullet;
    }

    private ObjectPool<Bullet> CreateBulletPool(IReadOnlyList<Collider> ignoreColliders = null, string teamTag = null)
    {
        var pool = new ObjectPool<Bullet>(
            createFunc: () => CreateBullet(ignoreColliders, teamTag),
            actionOnGet: bullet =>
            {
                bullet.gameObject.SetActive(true);
            },
            actionOnRelease: bullet =>
            {
                bullet.gameObject.SetActive(false);
            },
            actionOnDestroy: bullet =>
            {
                if (bullet != null)
                    Destroy(bullet.gameObject);
            },
            collectionCheck: true,
            defaultCapacity: _defaultBulletSize,
            maxSize: _maxBulletCapacity
        );

        return pool;
    }
}
