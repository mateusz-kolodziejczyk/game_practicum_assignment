using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public abstract float Health{ get; }

    public abstract void Attack();
    public abstract void TakeDamage(float damageAmount);
    public void Die()
    {
        Destroy(gameObject);
    }
}
