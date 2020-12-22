using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRanged : Enemy
{
    // Serialized variables
    [SerializeField]
    float health = 100;
    [SerializeField]
    public float damage = 10;
    [SerializeField]
    public float timeBetweenAttacks = 1f;
    [SerializeField]
    AudioClip awareNoise;
    [SerializeField]
    int score;
    [SerializeField]
    Material enemyDamagedMaterial;
    [SerializeField]
    GameObject healthBar;
    [SerializeField]
    public GameObject bullet;
    [SerializeField]
    public Transform bulletEmitter;
    [SerializeField]
    float bulletSpread;
    [SerializeField]
    public AudioClip ShootingNoise;

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

    public override Transform Target { get; set; }

    public bool isAttacking = true;
    private IEnumerator attackCoroutine;
    public float attackTimer;
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private IEnumerator damagedColorCoroutine;
    private RangedAI ai;
    public AnimationClip attackAnimation;



    private void Awake()
    {
        ai = GetComponent<RangedAI>();
        Target = ai.target;
        MaxHealth = health;
        enemyRenderer = GetComponentInChildren<Renderer>();
        originalMaterial = enemyRenderer.material;

        attackTimer = timeBetweenAttacks * 0.5f;
        TimeBetweenAttacks = timeBetweenAttacks;
        EnemyAudioSource = GetComponent<AudioSource>();
        GamesManager = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();

    }
    public override void Update()
    {
        if (Target == null)
        {
            Target = ai.target;
        }
        base.Update();
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
                var instantiatedBullet = Instantiate(bullet, bulletEmitter.transform.position, bulletEmitter.transform.rotation);
                var updatedTargetPosition = Target.position;
                updatedTargetPosition.y += 1.5f;
                instantiatedBullet.GetComponent<Rigidbody>().velocity = (updatedTargetPosition - bulletEmitter.transform.position).normalized * 20 + CalculateSpread(transform);
                var bulletScript = instantiatedBullet.GetComponent<Bullet>();
                // 20% chance of shooting a bigger and more powerful bullet
                int attackType = Random.Range(0, 5);

                if(attackType == 0)
                {
                    instantiatedBullet.transform.localScale *= 4;
                    bulletScript.Damage = damage*2;
                }
                else
                {

                    instantiatedBullet.transform.localScale *= 2;
                    bulletScript.Damage = damage;
                }
                bulletScript.IsFriendly = false;
                attackTimer = 0.0f;
                EnemyAudioSource.clip = ShootingNoise;
                EnemyAudioSource.Play();
            }
            yield return null;
        }
    }

    public void StartStopAttack(bool attack)
    {
        // only start coroutine if not attacking
        if (attack)
        {
            attackCoroutine = Attack(Target.GetComponent<Player>());
            StartCoroutine(attackCoroutine);
        }
        else
        {
            attackTimer = timeBetweenAttacks * 0.5f;
        }
        isAttacking = attack;
    }

    public Vector3 CalculateSpread(Transform characterTransform)
    {
        float upSpread = Random.Range(-0.5f, 0.5f);
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
