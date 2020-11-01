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
    private bool hasSeenThePlayer = false;

    Enemy enemy;
    AudioSource enemyAudioSource;
    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<BasicMovement>();
        target = GameObject.FindWithTag("Player").transform;

        agent.updateRotation = false;
        agent.updatePosition = true;

        enemy = GetComponent<Enemy>();
        enemyAudioSource = GetComponent<AudioSource>();
    }


    private void Update()
    {
        if (CanSeePlayer() || hasSeenThePlayer)
        {
            if (target != null)
            {
                agent.SetDestination(target.position);

            }

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
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
                if (hit.collider.CompareTag("Player"))
                {
                if (!hasSeenThePlayer)
                {
                    enemy.playAwareNoise();
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
