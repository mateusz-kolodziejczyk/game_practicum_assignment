using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutomaticWeapon : Weapon
{
    public abstract IEnumerator ShootAutomatic(Camera camera, GameObject character, AudioSource audioSource);
}
