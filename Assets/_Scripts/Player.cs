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
    [SerializeField]
    private AudioClip hurtNoise;
    public float Health { get; set; }

    private Text healthText;

    private GameManagement gameManagement;
    private AudioSource playerSource;

    private void Awake()
    {
        playerSource = GetComponent<AudioSource>();
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        healthText = GameObject.FindWithTag("HealthText").GetComponent<Text>();
    }

    public void LowerHealth(float amountToChange)
    {
        health -= amountToChange;
        playerSource.clip = hurtNoise;
        playerSource.Play();
        healthText.text = "Health: " + health;
        if (health <= 0)
        {
            gameManagement.ChangeLives(false);
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
