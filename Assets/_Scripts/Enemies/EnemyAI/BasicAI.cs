using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Basic Ai and Basic Movement are based on the standard assets AI and third person classes
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(BasicMovement))]
public class BasicAI : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public BasicMovement character { get; private set; } // the character we are controlling
    public Transform target;                                    // target to aim for
    public float maxSeeDistance = 50;
    private bool hasSeenThePlayer = false;

    Enemy enemy;
    AudioSource enemyAudioSource;
    EnemyHandleFSM fsmHandler;

    private Vector3 startPosition;
    private float movingRange;
    private void Start()
    {
        startPosition = transform.position;
        startPosition.y = 0;
        movingRange = maxSeeDistance * 2f;
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<BasicMovement>();
        target = GameObject.FindWithTag("Player").transform;
        fsmHandler = GetComponent<EnemyHandleFSM>();

        agent.updateRotation = false;
        agent.updatePosition = true;

        enemy = GetComponent<Enemy>();
        enemyAudioSource = GetComponent<AudioSource>();
    }

 
    private void Update()
    {
        if ((CanSeePlayer() || hasSeenThePlayer))
        {
            // If the enemy is back at spawn after seeing the player, revert the enemy to nto having seen the player and make it play the idle animation again
            if (Vector3.Distance(startPosition, transform.position) <= 3 && Vector3.Distance(agent.destination, startPosition) <= 0.5) 
            {
                fsmHandler.IsMoving(false);
                hasSeenThePlayer = false;
            }


            // Only set a new position if the player is within the start position range
            if (target != null && Vector3.Distance(startPosition, target.position) <= movingRange)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                agent.SetDestination(startPosition);
            }

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false);
            }
            else
            {
                character.Move(Vector3.zero, false);
            }
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
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
                    fsmHandler.IsMoving(true);
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
