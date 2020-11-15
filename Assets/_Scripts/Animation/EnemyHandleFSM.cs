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
    List<Transform> waypointTransforms = new List<Transform>();
    [SerializeField]
    float maxWayPointRange = 10;

    float originalSpeed;

    BasicMovement basicMovement;

    NavMeshAgent agent;

    BasicAI characterAI;


    void Start()
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
            waypointTransforms.Add(waypoint.transform);
        }

        var wayPointTransformsToRemove = new List<Transform>();
        // Remove waypoints that are too far away 
        foreach (var waypointTransform in waypointTransforms)
        {
            if (!(Vector3.Distance(waypointTransform.position, transform.position) <= maxWayPointRange) || !WaypointInLOS(waypointTransform))
            {
                wayPointTransformsToRemove.Add(waypointTransform);
            }

        }
        waypointTransforms.RemoveAll(x => wayPointTransformsToRemove.Contains(x));
        foreach (var x in waypointTransforms)
        {
            Debug.Log(x);
        }

        if (waypointTransforms.Count > 0)
        {

            waypointTransforms.Sort((a, b) => (Vector3.Distance(a.position, transform.position).CompareTo(Vector3.Distance(b.position, transform.position))));
            IsPatrolling(true);
        }
    }

    public void IsMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isPatrolling", false);
        agent.speed = originalSpeed;
    }

    public void IsPatrolling(bool isPatrolling)
    {
        if (waypointTransforms.Count > 0)
        {
            anim.SetBool("isPatrolling", isPatrolling);
            agent.speed = originalSpeed * 0.2f;
        }
        anim.SetBool("isMoving", false);
    }

    private void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo((0));
        if (info.IsName("Patrol") && waypointTransforms.Count > 0)
        {
            var waypointTransformPosition = waypointTransforms[WPIndex].position;
            agent.isStopped = false;
            agent.destination = waypointTransformPosition;
            basicMovement.Move(agent.desiredVelocity, false);
            if (Vector3.Distance(transform.position, waypointTransformPosition) < 4 && waypointTransforms.Count > 1)
            {
                // New start position is the waypoint but on the floor(y = 0)
                characterAI.StartPosition = new Vector3(waypointTransformPosition.x, 0, waypointTransformPosition.z);
                int previousIndex = WPIndex;
                int newIndex; do
                {
                    newIndex = Random.Range(0, waypointTransforms.Count);

                } while (newIndex == previousIndex);
                WPIndex = newIndex;
            }
            else if(waypointTransforms.Count == 1)
            {
                IsPatrolling(false);
            }
        }
    }

    private bool WaypointInLOS(Transform waypointTransform)
    {
        // Created using help from https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
        var directionToWaypoint = waypointTransform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToWaypoint, out RaycastHit hit))
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
