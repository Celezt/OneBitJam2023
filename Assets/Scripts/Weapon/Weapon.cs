using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Weapon", menuName = "Data/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public GameObject bulletPrefab;
    public Perk[] powerUp;
    public float bulletSpread;
    public int bulletAmount;
    public float bulletSpeed;
    public float fireDelay;
    public bool isAutomatic;
    public bool doDoT;

    public void Shoot(Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            Vector3 pDirection = direction + new Vector3(Random.Range(-bulletSpread, bulletSpread), 0, Random.Range(-bulletSpread, bulletSpread));
            GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.LookRotation(pDirection, Vector3.up));
            bullet.GetComponent<Bullet>().weapon = this;
            bullet.GetComponent<Bullet>().doT = doDoT;
        }
    }
}
