﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    // Private members
    [SerializeField]
    private int ammoPerShot = 1;
    [SerializeField]
    private int maxAmmo = 100;
    [SerializeField]
    private int bulletsPerShot = 10;
    [SerializeField]
    private float bulletSpread = 1;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float timeBetweenShots;

    [SerializeField]
    private GameObject bulletEmitter;

    // Audio
    [SerializeField]
    private AudioClip shootingNoise;

    // The range the ray trace is set to
    [SerializeField]
    private float accurateRange;


    // Properties
    public override float BulletSpread { get { return bulletSpread; } set { bulletSpread = value; } }

    public override int WeaponID { get; set; } = 3;
    public override int MaxAmmo { get { return maxAmmo; } set { maxAmmo = value; } }

    // Methods
    private void Start()
    {
    }
    public override void ShootSingle(Camera camera, GameObject character, AudioSource audioSource)
    {
        // Using code from https://answers.unity.com/questions/1582934/how-to-make-bullet-go-straight-to-middle-of-the-sc.html

        // Create a ray from the camera going through the middle of your screen
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        // Check whether your are pointing to something so as to adjust the direction
        Vector3 targetPoint;

        targetPoint = ray.GetPoint(25);


        // Create the bullet and give it a velocity according to the target point computed before
        var characterVelocity = character.GetComponent<Rigidbody>().velocity;

        var addedVelocity = new Vector3(0, 0, 0);
        var forwardCharacterVelocity = Vector3.Dot(characterVelocity, character.transform.forward);

        // Only set the added velocity to a value if the character is not moving backwards(to stop the bullets from losing velocity)
        if (forwardCharacterVelocity > 0.1)
        {
            addedVelocity = character.transform.forward * forwardCharacterVelocity;
        }
        
        for(int i = 0; i < bulletsPerShot; i++)
        {
            // Lower ammo each time it's shot
            var instantiatedBullet = Instantiate(bulletPrefab, bulletEmitter.transform.position, bulletEmitter.transform.rotation);

            var bulletDirection = (targetPoint - bulletEmitter.transform.position).normalized * 20 + addedVelocity + CalculateSpread(character.transform);
            instantiatedBullet.GetComponent<Rigidbody>().velocity = bulletDirection;

            var bulletScript = instantiatedBullet.GetComponent<Bullet>();

            bulletScript.IsFriendly = true;
            bulletScript.Damage = damage;
        }
        // Ammo
        CurrentAmmo--;
        setAmmoText();

        // Audio
        audioSource.clip = shootingNoise;
        audioSource.Play();

    }
}
