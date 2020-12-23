using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBoss : BasicRanged 
{
    GameManagement gameManagement;
    [SerializeField]
    GameObject droppableItem;
    [SerializeField]
    int bulletsPerShot;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    public override void Die()
    {
        gameManagement.OpenLevelExit();
        gameManagement.BeatGame = true;
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        Instantiate(droppableItem, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
    }

    public override IEnumerator Attack(Player player)
    {
        isAttacking = true;
        // Attack timer stays constant even if the enemy disengages
        while (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeBetweenAttacks)
            {
                for (int i = 0; i < bulletsPerShot; i++)
                {
                    var instantiatedBullet = Instantiate(bullet, bulletEmitter.transform.position, bulletEmitter.transform.rotation);
                    var targetVelocity = Target.GetComponent<Rigidbody>().velocity;
                    var updatedTargetPosition = Target.position + targetVelocity*0.5f;
                    updatedTargetPosition.y += 1.5f;
                    instantiatedBullet.GetComponent<Rigidbody>().velocity = (updatedTargetPosition - bulletEmitter.transform.position).normalized * 30 + CalculateSpread(transform);
                    var bulletScript = instantiatedBullet.GetComponent<Bullet>();
                    bulletScript.IsFriendly = false;
                    instantiatedBullet.transform.localScale *= 3;
                }

                attackTimer = 0.0f;
                EnemyAudioSource.clip = ShootingNoise;
                EnemyAudioSource.Play();
            }
            yield return null;
        }
    }
}
