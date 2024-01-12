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
        set 
        {
            var newWeapon = value;

            if (newWeapon != _weapon)
            {
                if (_weapon != null)
                    _weapon.Handler = null;

                if (newWeapon != null)
                    newWeapon.Handler = this;
            }

            _weapon = newWeapon;
        }
    }
    public IReadOnlyList<Collider> IgnoreColliders => _ignoreColliders;
    public string TeamTag => _teamTag;

    [SerializeField]
    private string _teamTag;
    [SerializeField]
    private Weapon _weapon;

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private Collider[] _ignoreColliders;

    private float _lastFiredTime;
    private bool _isShooting;

    public void OnShoot()
    {
        if (_weapon == null)
            return;

        if ((Time.time - _lastFiredTime) > _weapon.Cooldown)
        {
            _weapon.Shoot();
            _lastFiredTime = Time.time;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (_weapon == null)
            return;

        if (_weapon.IsAutomatic)
        {
            _isShooting = context.performed;
        }
        else if(context.started)
        {
            OnShoot();
        }
    }

    private void Update()
    {
        if (_isShooting)
        {
            OnShoot();
        }
    }
}
