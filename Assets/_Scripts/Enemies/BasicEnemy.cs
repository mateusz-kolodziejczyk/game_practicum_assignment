using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    [SerializeField]
    float health = 100;
    [SerializeField]
    float damage = 10;
    [SerializeField]
    float timeBetweenAttacks = 1f;
    [SerializeField]
    AudioClip awareNoise;
 
    public override float Health { get { return health; } }
    public override float Damage { get { return damage; } }
    public override float TimeBetweenAttacks{ get { return timeBetweenAttacks; } }
    public override AudioClip AwareNoise { get; set; }
    public override AudioSource EnemyAudioSource { get; set; }
    public override GameManagement GamesManager { get; set; }
    public override int Score { get; set; } = 50;

    private bool isAttacking = false;
    private IEnumerator attackCoroutine;

    private void Awake()
    {
        EnemyAudioSource = GetComponent<AudioSource>();
        GamesManager = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        
    }
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


    public override void playAwareNoise()
    {
        EnemyAudioSource.clip = awareNoise;
        EnemyAudioSource.Play();
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
