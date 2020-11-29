using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRanged : Enemy
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
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    Transform bulletEmitter;
    [SerializeField]
    float bulletSpread;

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

    // Private variables
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;
    private float attackTimer;
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private IEnumerator damagedColorCoroutine;
    private BasicAI ai;
    private Transform target;
    public AnimationClip attackAnimation;



    private void Awake()
    {
        ai = GetComponent<BasicAI>();
        target = ai.target;
        MaxHealth = health;
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
                var instantiatedBullet = Instantiate(bullet, bulletEmitter.transform.position, bulletEmitter.transform.rotation);
                instantiatedBullet.GetComponent<Rigidbody>().velocity = (target.position - bulletEmitter.transform.position).normalized * 20 + CalculateSpread(transform);
                var bulletScript = instantiatedBullet.GetComponent<Bullet>();

                bulletScript.IsFriendly = false;
                bulletScript.Damage = damage;
                player.LowerHealth(damage);
                attackTimer = 0.0f;
            }
            yield return null;
        }
    }

    public void StartStopAttack(bool attack)
    {
        // only start coroutine if not attacking
        if (!isAttacking && attack)
        {
            attackCoroutine = Attack(ai.target.GetComponent<Player>());
        }
        isAttacking = attack;
    }

    public Vector3 CalculateSpread(Transform characterTransform)
    {
        float upSpread = Random.Range(-1f, 1f);
        float rightSpread = Random.Range(-1f, 1f);

        var rightAdded = characterTransform.right.normalized * rightSpread * bulletSpread;
        var upAdded = characterTransform.up.normalized * upSpread * bulletSpread;

        return rightAdded + upAdded;
    }

    public override void PlayAwareNoise()
    {
        EnemyAudioSource.clip = awareNoise;
        EnemyAudioSource.Play();
    }
}
