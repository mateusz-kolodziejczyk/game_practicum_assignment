using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract int MaxAmmo { get; }
    public abstract int AmmoPerShot { get; }
    public abstract int BulletsPerShot { get; }
    public abstract int BulletSpread { get; }
    public abstract float Damage { get; }
    public abstract GameObject Bullet {get;}
    public abstract GameObject BulletEmitter {get;}

    // Character is needed to calculate the speed to add to the bullet when fired
    public abstract void Shoot(Camera camera, GameObject character);

}
