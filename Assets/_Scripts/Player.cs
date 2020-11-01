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

    public void ChangeHealth(float amountToChange)
    {
        health -= amountToChange;
        healthText.text = "Health: " + health;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(1);
    }
}
