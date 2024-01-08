using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

[HideMonoScript]
[CreateAssetMenu(fileName = "Weapon", menuName = "Data/Weapon", order = 1)]
public class Weapon : ScriptableObject
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
    public float Spread
    {
        get => _spread;
        set => _spread = value;
    }
    public int Amount
    {
        get => _amount;
        set => _amount = Mathf.Max(value, 0);
    }
    public float Cooldown
    {
        get => _cooldown;
        set => _cooldown = Mathf.Max(value, 0);
    }
    public bool IsAutomatic
    {
        get => _isAutomatic;
        set => _isAutomatic = value;
    }

    public event Action<GameObject> OnBulletChangeCallback = delegate { };

    [SerializeField, AssetsOnly]
    private GameObject _bulletPrefab;
    [SerializeField, Indent, MinValue(0)]
    private int _maxBulletCapacity = 100;
    [SerializeField, Indent(2), MinValue(0), MaxValue("@_maxBulletCapacity")]
    private int _defaultBulletSize = 10;

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private float _spread = 0;
    [SerializeField, MinValue(0)]
    private int _amount = 1;
    [SerializeField, MinValue(0)]
    private float _cooldown = 0.2f;

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private bool _isAutomatic;

    public Bullet CreateBullet(Collider[] ignoreColliders = null, string teamTag = null)
    {
        if (BulletPrefab == null)
            return null;

        Bullet bullet = Instantiate(_bulletPrefab).GetComponent<Bullet>();
        bullet.IgnoreCollisions(ignoreColliders);
        bullet.TeamTag = teamTag;

        return bullet;
    }

    public ObjectPool<Bullet> CreateBulletPool(Collider[] ignoreColliders = null, string teamTag = null)
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

    public void Shoot(Vector3 position, Quaternion rotation, ObjectPool<Bullet> pool)
    {
        if (_bulletPrefab == null)
            return;

        for (int i = 0; i < _amount; i++)
        {
            Quaternion spreadRotation = rotation * Quaternion.Euler(UnityEngine.Random.Range(-_spread, _spread), UnityEngine.Random.Range(-_spread, _spread), 0);

            Bullet bullet = pool.Get();
            bullet.Pool = pool;
            bullet.Shoot(position, spreadRotation);
        }
    }

    public void Shoot(Vector3 position, Quaternion rotation, Collider[] ignoreColliders = null)
    {
        if (_bulletPrefab == null) 
            return;

        for (int i = 0; i < _amount; i++)
        {
            Quaternion spreadRotation = rotation * Quaternion.Euler(UnityEngine.Random.Range(-_spread, _spread), 0, UnityEngine.Random.Range(-_spread, _spread));

            GameObject gameObject = Instantiate(_bulletPrefab, position, spreadRotation);
            Bullet bullet = gameObject.GetComponent<Bullet>();
            bullet.IgnoreCollisions(ignoreColliders);
        }
    }
}
