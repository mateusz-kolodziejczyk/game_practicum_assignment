using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage { get; set; } = 10;

    public bool IsFriendly { get; set; } = false;

    // Destroy the bullet after 3 seconds
    void Start()
    {
        Destroy(gameObject, 3);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy") && IsFriendly)
        {
            collider.gameObject.GetComponent<Enemy>().TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (collider.tag == "Player")
        {
            if (IsFriendly)
            {
                // Dont damage player
            }
            else
            {
                // 
            }
        }
        else if (collider.CompareTag("Collectable"))
        {
           // Don't destroy if it hits a collectable
        }
        else if (collider.CompareTag("PlayerBullet"))
        {
            // Don't destroy if it hits a player bullet
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
