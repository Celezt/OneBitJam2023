using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

[HideMonoScript]
public class WeaponHandler : MonoBehaviour
{
    public Weapon Weapon
    {
        get => _weapon;
        set => SetWeapon(value);
    }

    [SerializeField]
    private string _teamTag;
    [SerializeField]
    private Weapon _weapon;
    [SerializeField, Indent]
    private Vector3 _offset;
    [SerializeField, Indent]
    private Quaternion _rotation = Quaternion.identity;

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private Collider[] _ignoreColliders;

    private float _lastFiredTime;
    private bool _isShooting;

    private ObjectPool<Bullet> _bulletPool;

    public void OnShoot()
    {
        if((Time.time - _lastFiredTime) > _weapon.Cooldown)
        {
            Vector3 worldOffset = transform.TransformDirection(_offset);
            _weapon.Shoot(transform.position + worldOffset, transform.rotation * _rotation, _bulletPool);
            _lastFiredTime = Time.time;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (_weapon.IsAutomatic)
        {
            _isShooting = context.performed;
        }
        else if(context.started)
        {
            OnShoot();
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        Weapon previousWeapon = _weapon;
        _weapon = weapon;

        if (weapon == null)
            return;

        if (weapon != previousWeapon)   // Not the same weapon.
        {
            // Create a bullet pool for the current used weapon.
            void CreateBulletPool()
            {
                if (_bulletPool != null)
                    _bulletPool.Dispose();

                _bulletPool = weapon.CreateBulletPool(_ignoreColliders, _teamTag);
            }

            CreateBulletPool();
            weapon.OnBulletChangeCallback += _ => CreateBulletPool();   // Create new bullet pool if bullet is changed.
        }
    }

    private void Start()
    {
        if (_weapon)    // Create a bullet pool for the current used weapon.
        {
            _bulletPool = Weapon.CreateBulletPool(_ignoreColliders, _teamTag);
        }
    }

    private void Update()
    {
        if (_isShooting)
        {
            OnShoot();
        }
    }

    private void OnDestroy()
    {
        if (_bulletPool != null)
            _bulletPool.Dispose();
    }
}
