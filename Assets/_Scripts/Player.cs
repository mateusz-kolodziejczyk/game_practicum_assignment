using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    public float Health { get; set; }

    [SerializeField]
    private Text healthText;

    private GameManagement gameManagement;

    private void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    public void ChangeHealth(float amountToChange)
    {
        health -= amountToChange;
        healthText.text = "Health: " + health;
        if (health <= 0)
        {
            gameManagement.changeLives(false);
            Die();
        }
    }

    private void Die()
    {
        // If more lives remain, reset
        if (gameManagement.CurrentLives > 0)
        {
            gameManagement.ReloadLevel();
        }
        // Else game over
        else
        {
            gameManagement.LoadGameOverScreen();
        }
    }
}
