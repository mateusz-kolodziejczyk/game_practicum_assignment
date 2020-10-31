using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameManagement gameManagement;
    [SerializeField]
    private int score = 1;
    void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManagement.addToScore(score);
            Debug.Log(gameManagement.PlayerScore);
            Destroy(gameObject);
        }
    }
}
