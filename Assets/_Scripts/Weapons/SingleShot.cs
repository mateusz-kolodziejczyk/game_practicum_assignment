﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShot : Weapon
{
    // Private members
    [SerializeField]
    private int ammoPerShot = 1;
    [SerializeField]
    private int maxAmmo = 100;
    [SerializeField]
    private int bulletsPerShot = 1;
    [SerializeField]
    private float bulletSpread = 0;
    [SerializeField]
    private float damage = 20;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float timeBetweenShots;

    [SerializeField]
    private GameObject bulletEmitter;

    // Audio
    [SerializeField]
    private AudioClip shootingNoise;


    // Properties
    public override float BulletSpread { get { return bulletSpread; } set { bulletSpread = value; } }
    public override int WeaponID { get; set; } = 1;



    // Methods
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
        var instantiatedBullet = Instantiate(bulletPrefab, bulletEmitter.transform.position, bulletEmitter.transform.rotation);
        instantiatedBullet.GetComponent<Rigidbody>().velocity = (targetPoint - bulletEmitter.transform.position).normalized * 20 + addedVelocity;
        var bulletScript = instantiatedBullet.GetComponent<Bullet>();

        bulletScript.IsFriendly = true;
        bulletScript.Damage = damage;

        // Audio
        audioSource.clip = shootingNoise;
        audioSource.Play();

    }
}
