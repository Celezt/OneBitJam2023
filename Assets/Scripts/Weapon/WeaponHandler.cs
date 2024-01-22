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
    public Rigidbody MoveRigidbody => _moveRigidbody;
    public string TeamTag => _teamTag;
    public float DurationUsed => Time.time - _onUseTime;
    public bool IsUsed
    {
        get => _isUsing;
        set => _isUsing = value;
    }

    [SerializeField]
    private string _teamTag;
    [SerializeField]
    private Weapon _weapon;
    [SerializeField]
    private Rigidbody _moveRigidbody;

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private Collider[] _ignoreColliders;

    private float _onUseTime;
    private float _lastOnUsedTime;
    private bool _isUsing;

    public void OnUse()
    {
        if (_weapon == null)
            return;

        if ((Time.time - _lastOnUsedTime) > _weapon.Cooldown)
        {
            _weapon.Use();
            _lastOnUsedTime = Time.time;
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (_weapon == null)
            return;

        if (_weapon.IsAutomatic)
        {
            _isUsing = context.performed;
            _onUseTime = Time.time;
        }
        else if(context.started)
            OnUse();
    }

    private void Update()
    {
        if (_isUsing)
            OnUse();
    }
}
