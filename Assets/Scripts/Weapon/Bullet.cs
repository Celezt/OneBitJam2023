using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public string enemyTag;
    [SerializeField]
    public Weapon weapon;
    [SerializeField]
    private float lifespan;
    [SerializeField]
    private float bulletDamage;

    public float damageMultiplier = 1;
    public float doTTimer = 10;
    [HideInInspector]
    public bool doT = false;

    private void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan < 0)
        {
            Destroy(gameObject);
        }

        this.transform.position += (transform.forward * weapon.bulletSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(enemyTag) && other.gameObject.GetComponent<EnemyHealth>() != null)
        {
            Debug.Log("Hit " + other.name);
            other.gameObject.GetComponent<EnemyHealth>().DoDamage(bulletDamage * damageMultiplier);
            if (doT)
            {
               other.gameObject.GetComponent<EnemyHealth>().DamageOverTime(3, doTTimer, bulletDamage);
            }
            Destroy(this.gameObject);
        }
    }
}
