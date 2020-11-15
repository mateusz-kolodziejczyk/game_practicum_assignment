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
    [SerializeField]
    Material enemyDamagedMaterial;

    // Properties
    public override AudioSource EnemyAudioSource { get; set; }
    public override GameManagement GamesManager { get; set; }
    public override int Score { get { return score; } set { score = value; } }
    public override float Damage { get { return damage; } set { damage = value; } }
    public override float TimeBetweenAttacks { get { return timeBetweenAttacks; } set { timeBetweenAttacks = value; } }
    public override float Health { get { return health; } set { health = value; } }
    public override Material EnemyDamagedMaterial { get { return enemyDamagedMaterial; } set { enemyDamagedMaterial = value; } }
    public override Material OriginalMaterial { get { return originalMaterial; } set { originalMaterial = value; } }
    public override Renderer EnemyRenderer { get { return enemyRenderer; } set { enemyRenderer = value; } }
    public override IEnumerator DamagedColorCoroutine { get { return damagedColorCoroutine; } set { damagedColorCoroutine = value; } }

    // Private variables
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;
    private float attackTimer;
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private IEnumerator damagedColorCoroutine;


    private void Awake()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
        originalMaterial = enemyRenderer.material;

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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Collision");
            if (!isAttacking)
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
            Debug.Log("Collision Exit");
            isAttacking = false;
        }
    }
}
