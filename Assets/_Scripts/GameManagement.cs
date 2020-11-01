using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance;
    public int PlayerScore { get; set; } = 0;
    [SerializeField]
    Text itemProgressText;
    Text scoreText;
    Text muteText;

    int itemProgress;
    bool isMute = false;
    
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

    private void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            Mute();
        }
    }

    public void addToScore(int scoreToAdd)
    {
        PlayerScore += scoreToAdd;
        setScoreText();
    }

    public void addToItemProgress()
    {
        itemProgress++;
        itemProgressText.text = "Items Found: " + itemProgress + "/4";
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


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("OnDisable");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Using help from https://answers.unity.com/questions/1580211/when-an-object-is-dontdestroyonload-where-can-i-pu.html
        if (scene.isLoaded)
        {
            itemProgressText = GameObject.FindWithTag("ItemProgressText").GetComponent<Text>();
            scoreText = GameObject.FindWithTag("Score Text").GetComponent<Text>();
            muteText = GameObject.FindWithTag("MuteText").GetComponent<Text>();
            updateMuteText();
            setScoreText();
            itemProgress = 0;
        }
    }

    void updateMuteText()
    {
        // If muted, say that the audio is muted. If not don't
        string mutedStatus = isMute ? "Muted" : "Active";
        muteText.text = "Mute: 'M'\nAudio: " + mutedStatus;
    }

    void setScoreText()
    {
        scoreText.text = "Score: " + PlayerScore;
    }

    void Mute()
    {
        // Done using help from https://answers.unity.com/questions/829987/how-to-make-mute-button.html
        isMute = !isMute;
        AudioListener.volume = isMute ? 0 : 1;
        updateMuteText();
    }
}
