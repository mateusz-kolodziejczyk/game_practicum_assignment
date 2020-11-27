using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject projectile;     // Start is called before the first frame update
    public GameObject target;
    [SerializeField]
    private float maxDistance = 30;
    private float shotTimer = 0f;
    [SerializeField]
    private float force;
    [SerializeField]
    private GameObject shotEmitter;

    [SerializeField]
    private float timeBetweenShots = 2f;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player");
    }
    void Update()
    {
        if (CanSeePlayer())
        {
            transform.LookAt(target.transform);
            shotTimer += Time.deltaTime;
            if (shotTimer >= timeBetweenShots)
            {
                GameObject t = Instantiate(projectile, shotEmitter.transform.position, Quaternion.identity);
                Destroy(t, 3);
                t.GetComponent<Rigidbody>().AddForce(transform.forward * force);

                shotTimer = 0.0f;
            }
        }
    }

    bool CanSeePlayer()
    {
        // Created using help from https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
        var directionToPlayer = target.transform.position - shotEmitter.transform.position;
        if (Physics.Raycast(shotEmitter.transform.position, directionToPlayer, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player") && hit.distance < maxDistance)
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
