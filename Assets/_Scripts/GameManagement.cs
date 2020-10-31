using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance;
    public int PlayerScore { get; set; } = 0;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void addToScore(int scoreToAdd)
    {
        PlayerScore += scoreToAdd;
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void loadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel + 1);
    }
}
