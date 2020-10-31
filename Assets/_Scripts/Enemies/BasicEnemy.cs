using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    float health = 100;
    float damage = 10;
    float timeBetweenAttacks = 1.5f;
    public override float Health { get { return health; } }
    public override float Damage { get { return damage; } }
    public override float TimeBetweenAttacks{ get { return timeBetweenAttacks; } }
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;
    
    public override IEnumerator Attack(Player player)
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        while (isAttacking)
        {
            player.ChangeHealth(damage);
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health < 0.0f)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAttacking)
        {
            if (collision.collider.CompareTag("Player"))
            {
                attackCoroutine = Attack(collision.collider.GetComponent<Player>());
                StartCoroutine(attackCoroutine);
            }
        }
   
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isAttacking = false;
            StopCoroutine(attackCoroutine);
        }
    }

}
