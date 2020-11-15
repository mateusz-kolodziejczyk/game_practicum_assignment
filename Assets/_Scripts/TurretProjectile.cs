using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float damage = 10;
   

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().LowerHealth(damage);
            Destroy(gameObject);
        }
    }

   
}
