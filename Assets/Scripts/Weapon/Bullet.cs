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
        if(other.gameObject.CompareTag(enemyTag) && other.gameObject.GetComponent<Health>() != null)
        {
            Debug.Log("Hit " + other.name);
            if (bulletBehaviour.doT)
            {
                other.gameObject.GetComponent<Health>().DoDOTDamage(1, 2, 2, 1);
            }
            else
            {
                other.gameObject.GetComponent<Health>().DoDamage(bulletDamage * bulletBehaviour.damageMultiplier); 
            }
            Destroy(this.gameObject);
        }
    }
}
