using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandleFSM : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    AnimatorStateInfo info;

    int WPIndex = 0;
    [SerializeField]
    float maxWayPointRange = 30;

    float originalSpeed;

    BasicMovement basicMovement;

    NavMeshAgent agent;

    BasicAI characterAI;

    float attackAnimationLength = -1;

    public List<Transform> WaypointTransforms { get; set; } = new List<Transform>();

    public virtual void Start()
    {
        characterAI = GetComponent<BasicAI>();
        basicMovement = GetComponent<BasicMovement>();
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        // Find all waypoitns
        var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        // Only add waypoints that are close to the enemy so they don't run across the map.
        foreach (var waypoint in waypoints)
        {
            WaypointTransforms.Add(waypoint.transform);
        }

        var wayPointTransformsToRemove = new List<Transform>();
        // Remove waypoints that are too far away 
        foreach (var waypointTransform in WaypointTransforms)
        {
            if (!(Vector3.Distance(waypointTransform.position, transform.position) <= maxWayPointRange) || !WaypointInLOS(waypointTransform))
            {
                wayPointTransformsToRemove.Add(waypointTransform);
            }

        }
        WaypointTransforms.RemoveAll(x => wayPointTransformsToRemove.Contains(x));
        foreach (var x in WaypointTransforms)
        {
            Debug.Log(x);
        }

        if (WaypointTransforms.Count > 0)
        {

            WaypointTransforms.Sort((a, b) => (Vector3.Distance(a.position, transform.position).CompareTo(Vector3.Distance(b.position, transform.position))));
            IsPatrolling(true);
        }
    }

    public virtual void IsMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isPatrolling", false);
        anim.SetBool("isAttacking", false);
        agent.speed = originalSpeed;
    }

    public virtual void IsPatrolling(bool isPatrolling)
    {
        if (WaypointTransforms.Count > 0)
        {
            anim.SetBool("isPatrolling", isPatrolling);
            agent.speed = originalSpeed * 0.2f;
        }
        anim.SetBool("isMoving", false);
    }

    public virtual void IsAttacking(bool isAttacking)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", isAttacking);
    }
    public void SyncAttackSpeed(float attackTime, float animationTime)
    {

        Debug.Log(attackAnimationLength);

        anim.SetFloat("attackSpeedMultiplier", animationTime / attackTime);

    }
    public virtual void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo((0));
        if (info.IsName("Patrol") && WaypointTransforms.Count > 0)
        {
            var waypointTransformPosition = WaypointTransforms[WPIndex].position;
            agent.isStopped = false;
            agent.destination = waypointTransformPosition;
            basicMovement.Move(agent.desiredVelocity, false);
            if (Vector3.Distance(transform.position, waypointTransformPosition) < 4 && WaypointTransforms.Count > 1)
            {
                // New start position is the waypoint but on the floor(y = 0)
                    characterAI.StartPosition = new Vector3(waypointTransformPosition.x, 0, waypointTransformPosition.z);
                int previousIndex = WPIndex;
                int newIndex; do
                {
                    newIndex = Random.Range(0, WaypointTransforms.Count);

                } while (newIndex == previousIndex);
                WPIndex = newIndex;
            }
            else if (WaypointTransforms.Count == 1 && Vector3.Distance(transform.position, waypointTransformPosition) < 1)
            {
                IsPatrolling(false);
            }
        }
    }

    private bool WaypointInLOS(Transform waypointTransform)
    {
        // Created using help from https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
        var directionToWaypoint = waypointTransform.position - transform.position;
        int layerMask =~ LayerMask.GetMask("Enemy");
        if (Physics.Raycast(transform.position, directionToWaypoint, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.CompareTag("Waypoint"))
            {
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
