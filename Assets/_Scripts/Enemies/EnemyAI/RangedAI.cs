using UnityEngine;
// Basic Ai and Basic Movement are based on the standard assets AI and third person classes
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(BasicMovement))]
public class RangedAI : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public BasicMovement character { get; private set; } // the character we are controlling
    public Transform target;                                    // target to aim for
    public float maxSeeDistance = 50;
    private bool hasSeenThePlayer = false;

    BasicRanged enemy;
    AudioSource enemyAudioSource;
    RangedEnemyHandleFSM fsmHandler;

    private Vector3 startPosition;


    public Vector3 StartPosition { get { return startPosition; } set { startPosition = value; } }
    private void Start()
    {
        startPosition = transform.position;
        startPosition.y = 0;
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<BasicMovement>();
        target = GameObject.FindWithTag("Player").transform;
        fsmHandler = GetComponent<RangedEnemyHandleFSM>();

        agent.updateRotation = false;
        agent.updatePosition = true;

        enemy = GetComponent<BasicRanged>();
        enemyAudioSource = GetComponent<AudioSource>();
    }


    private void Update()
    {
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false);
        }
        else
        {
            character.Move(Vector3.zero, false);
        }

        // If it can see the player rotate the transform to point towards the player, as it will keep attacking
        if (CanSeePlayer())
        {
            transform.LookAt(target);
            transform.Rotate(new Vector3(0, 30, 0));
        }
        // If it stops seeing the player stop attacking
        else
        {
            fsmHandler.SetIdle();
            hasSeenThePlayer = false;
            enemy.StartStopAttack(false);
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    // This will handle the animation and stopping of the character
    public void Attack(float attackTime, bool isAttacking, float animationTime)
    {
        if (isAttacking)
        {
            fsmHandler.SetAttacking();
            fsmHandler.SyncAttackSpeed(attackTime, animationTime);
            // Set destination
        }
        else
        {
            fsmHandler.SetIdle();
        }
    }

    bool CanSeePlayer()
    {
        // Created using help from https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
        RaycastHit hit;
        var directionToPlayer = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit))
        {
            if (hit.collider.CompareTag("Player") && hit.distance < maxSeeDistance)
            {
                if (!hasSeenThePlayer)
                {
                    enemy.PlayAwareNoise();
                    enemy.StartStopAttack(true);
                    fsmHandler.SetAttacking();
                    hasSeenThePlayer = true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
