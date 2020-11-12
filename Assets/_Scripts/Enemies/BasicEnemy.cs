using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    // Serialized variables
    [SerializeField]
    float health = 100;
    [SerializeField]
    float damage = 10;
    [SerializeField]
    float timeBetweenAttacks = 1f;
    [SerializeField]
    AudioClip awareNoise;
    [SerializeField]
    int score;
 
    // Properties
    public override AudioSource EnemyAudioSource { get; set; }
    public override GameManagement GamesManager { get; set; }
    public override int Score { get { return score; } set { score = value; } }
    public override float Damage { get { return damage; } set { damage = value; } }
    public override float TimeBetweenAttacks{ get { return timeBetweenAttacks; } set { timeBetweenAttacks= value; } }
    public override float Health{ get { return health; } set { health= value; } }

    // Private variables
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;
    private float attackTimer;

    private void Awake()
    {
        attackTimer = timeBetweenAttacks - 0.1f;
        TimeBetweenAttacks = timeBetweenAttacks;
        EnemyAudioSource = GetComponent<AudioSource>();
        GamesManager = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        
    }
    public override IEnumerator Attack(Player player)
    {
        // Attack timer stays constant even if the enemy disengages
        while (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeBetweenAttacks)
            {
                player.ChangeHealth(damage);
                attackTimer = 0.0f;
            }
            yield return null;
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


    public override void PlayAwareNoise()
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
                isAttacking = true;
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
