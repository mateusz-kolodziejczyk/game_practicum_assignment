using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIncreasePickup : MonoBehaviour
{
    [SerializeField]
    float healthUp = 50;
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
            other.GetComponent<Player>().IncreaseMaxHealth(healthUp);
            Destroy(gameObject);
        }
    }
}
