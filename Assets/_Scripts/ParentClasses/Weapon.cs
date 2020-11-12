using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract float BulletSpread { get; set; }

    public abstract int WeaponID { get; set; }
    // Character is needed to calculate the speed to add to the bullet when fired
    // Audiosource is to allow the weapon manager to feed in their weapon, so that there is only one audiosource dedicated to weapons.
    public abstract void ShootSingle(Camera camera, GameObject character, AudioSource audioSource);

    public Vector3 CalculateSpread(Transform characterTransform)
    {
        float upSpread = Random.Range(-1f, 1f);
        float rightSpread = Random.Range(-1f, 1f);

        var rightAdded = characterTransform.right.normalized * rightSpread * BulletSpread;
        var upAdded = characterTransform.up.normalized * upSpread * BulletSpread;

        return rightAdded + upAdded;
    }

}
