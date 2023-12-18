using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth, currentHealth;
    float DoTDelay = 1;
    private CancellationTokenSource cTS;

    public void DoDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    private async UniTask DamageOverTimeASync(int nrOfAttacks, float delay, float damage, CancellationToken cT)
    {
        while (!cT.IsCancellationRequested || nrOfAttacks <= 0)
        {
            await UniTask.WaitForSeconds(delay);

            DoDamage(damage);

            nrOfAttacks--;
        }
    }
    
    public void DamageOverTime(int nrOfAttacks, float attackDelay, float damage)
    {
        if (cTS == null)
        {
            CTSUtility.Reset(ref cTS);
            DamageOverTimeASync(nrOfAttacks, attackDelay, damage, cTS.Token).ContinueWith(() => CTSUtility.Clear(ref cTS)).Forget();
        }
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref cTS);
    }
}
