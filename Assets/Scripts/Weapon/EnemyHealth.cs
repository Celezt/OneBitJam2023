using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth, currentHealth;

    public void DoDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public IEnumerator DoDOTDamage(float damage, float time, float startTime, float delay)
    {
        yield return new WaitForSeconds(delay);
        while(time <= 0)
        {
            currentHealth -= damage;
            time = startTime;
        }
    }
}
