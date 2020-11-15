using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float maxHealth = 100;
    public float Health { get; set; }

    [SerializeField]
    private Text healthText;

    private GameManagement gameManagement;

    private void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }

    public void LowerHealth(float amountToChange)
    {
        health -= amountToChange;
        healthText.text = "Health: " + health;
        if (health <= 0)
        {
            gameManagement.changeLives(false);
            Die();
        }
    }

    public void IncreaseHealth(float amountToChange)
    {
        health += amountToChange;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        healthText.text = "Health: " + health;
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
