using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy 
{
    float health = 100;
     public override float Health { get { return health; } }

    public override void Attack()
    {
       
    }

    public override void TakeDamage(float damageAmount)
    {
        health -= damageAmount; 
        if(health < 0.0f)
        {
            Die();
        }
    }


}
