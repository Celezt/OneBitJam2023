using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    public Weapon weapon;
    private float lastFiredTime;
    private bool isShooting;
    public void OnShoot()
    {
        if((Time.time - lastFiredTime) > weapon.firerate)
        {
            weapon.Shoot(this.transform.position, this.transform.forward);
            lastFiredTime = Time.time;
            Debug.Log("Enemy shot");
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
