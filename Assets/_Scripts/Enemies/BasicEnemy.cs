﻿using System.Collections;
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
    [SerializeField]
    Material enemyDamagedMaterial;
    [SerializeField]
    GameObject healthBar;

    // Properties
    public override AudioSource EnemyAudioSource { get; set; }
    public override GameManagement GamesManager { get; set; }
    public override int Score { get { return score; } set { score = value; } }
    public override float Damage { get { return damage; } set { damage = value; } }
    public override float TimeBetweenAttacks { get { return timeBetweenAttacks; } set { timeBetweenAttacks = value; } }
    public override float Health { get { return health; } set { health = value; } }
    public override float MaxHealth { get; set; }
    public override Material EnemyDamagedMaterial { get { return enemyDamagedMaterial; } set { enemyDamagedMaterial = value; } }
    public override Material OriginalMaterial { get { return originalMaterial; } set { originalMaterial = value; } }
    public override Renderer EnemyRenderer { get { return enemyRenderer; } set { enemyRenderer = value; } }
    public override IEnumerator DamagedColorCoroutine { get { return damagedColorCoroutine; } set { damagedColorCoroutine = value; } }
    public override GameObject HealthBar { get { return healthBar; } set { healthBar = value; } }
    public override Transform Target{ get; set; }

    // Private variables
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;
    private float attackTimer;
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private IEnumerator damagedColorCoroutine;
    private BasicAI ai;
    public AnimationClip attackAnimation;


    private void Awake()
    {
        ai = GetComponent<BasicAI>();
        Target = ai.target;
        MaxHealth = health;
        enemyRenderer = GetComponentInChildren<Renderer>();
        originalMaterial = enemyRenderer.material;

        attackTimer = timeBetweenAttacks*0.5f;
        TimeBetweenAttacks = timeBetweenAttacks;
        EnemyAudioSource = GetComponent<AudioSource>();
        GamesManager = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();

    }
    public override void Update()
    {
        if(Target == null)
        {
            Target = ai.target;
        }
        base.Update();
    }
    public override IEnumerator Attack(Player player)
    {
        // Attack timer stays constant even if the enemy disengages
        while (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeBetweenAttacks)
            {
                player.LowerHealth(damage);
                attackTimer = 0.0f;
            }
            yield return null;
        }
    }

    public override void PlayAwareNoise()
    {
        EnemyAudioSource.clip = awareNoise;
        EnemyAudioSource.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isAttacking)
            {

                ai.Attack(timeBetweenAttacks, true, attackAnimation.length);
                isAttacking = true;
                attackCoroutine = Attack(other.GetComponent<Player>());
                StartCoroutine(attackCoroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            attackTimer = timeBetweenAttacks * 0.5f;
            isAttacking = false;
            ai.Attack(timeBetweenAttacks, false, attackAnimation.length);
        }
    }
}
