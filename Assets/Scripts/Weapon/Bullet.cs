using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private List<BulletBehaviour> bulletBehaviours = new List<BulletBehaviour>();
    [SerializeField]
    private string enemyTag;
    [SerializeField]
    public Weapon weapon;
    [SerializeField]
    private float lifespan;

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
        if(other.gameObject.CompareTag(enemyTag))
        {
            Debug.Log("Hit " + other.name);
            Destroy(other.gameObject);
            for (int i = 0; i < bulletBehaviours.Count; i++)
            {
            }
        }
    }
}
