using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    private int currentAmmo;
    
    public float damage = 20;
    public Text AmmoText { get; set; }
    public abstract float BulletSpread { get; set; }

    public abstract int WeaponID { get; set; }
   
    public abstract int MaxAmmo { get; set; }
    public virtual int CurrentAmmo
    {
        get { return currentAmmo; }
        set
        {
            if (value > MaxAmmo)
            {
                currentAmmo = MaxAmmo;
            }
            else
            {
                currentAmmo = value;
            }
        }
    }
    // Character is needed to calculate the speed to add to the bullet when fired
    // Audiosource is to allow the weapon manager to feed in their weapon, so that there is only one audiosource dedicated to weapons.
    public virtual void Awake()
    {
        AmmoText = GameObject.FindWithTag("AmmoText").GetComponent<Text>();
        CurrentAmmo = MaxAmmo;
    }
    public abstract void ShootSingle(Camera camera, GameObject character, AudioSource audioSource);

    public Vector3 CalculateSpread(Transform characterTransform)
    {
        float upSpread = Random.Range(-1f, 1f);
        float rightSpread = Random.Range(-1f, 1f);

        var rightAdded = characterTransform.right.normalized * rightSpread * BulletSpread;
        var upAdded = characterTransform.up.normalized * upSpread * BulletSpread;

        return rightAdded + upAdded;
    }

    public void setAmmoText()
    {
        AmmoText.text = "Ammo: " + CurrentAmmo;
    }

    public void BoostDamage(float damageMultiplier)
    {
        StartCoroutine(IncreasingDamage(damageMultiplier));
    }

    private IEnumerator IncreasingDamage(float damageMultiplier)
    {
        damage *= damageMultiplier;
        yield return new WaitForSeconds(10);
        damage /= damageMultiplier;
    }

}
