using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    public Weapon weapon;
    private float lastFiredTime;
    private bool isShooting;
    public void OnShoot()
    {
        if((Time.time - lastFiredTime) > weapon.fireDelay)
        {
            weapon.Shoot(this.transform.position, this.transform.right);
            lastFiredTime = Time.time;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (weapon.isAutomatic)
        {
            isShooting = context.performed;
        }
        else if(context.started)
        {
            OnShoot();
        }
    }
    private void Update()
    {
        if (isShooting)
        {
            OnShoot();
        }
    }
}
