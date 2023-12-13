using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BulletBehaviour))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletBehaviour bulletBehaviour;
    public string enemyTag;
    [SerializeField]
    public Weapon weapon;
    [SerializeField]
    private float lifespan;
    [SerializeField]
    private float bulletDamage;

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
            if (bulletBehaviour.doT)
            {
                
            }
            else
            {
                other.gameObject.GetComponent<EnemyHealth>().DoDamage(bulletDamage * bulletBehaviour.damageMultiplier); 
            }
            Destroy(this.gameObject);
        }
    }
}
