using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamgeBoost : MonoBehaviour
{
    [SerializeField]
    // This is how much of the ammo is recovered for the current weapon
    float damageMultiplier = 1.2f;
    private GameManagement gameManagement;
    // Start is called before the first frame update
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManagement.BoostDamage(damageMultiplier);
            Destroy(gameObject);
        }
    }
}
