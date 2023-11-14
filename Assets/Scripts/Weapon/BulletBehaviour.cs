using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Data/Bullet", order = 1)]

public class BulletBehaviour : ScriptableObject
{
    public float damageMultiplier;
    public float doTTimer;
    public bool doT;
}
