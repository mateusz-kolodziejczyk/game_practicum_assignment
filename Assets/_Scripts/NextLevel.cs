using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    // Start is called before the first frame update

    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManagement.LoadNextLevel();
        }
    }
}
