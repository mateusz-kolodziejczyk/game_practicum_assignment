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

    GameObject levelExitDoor;
    GameObject bossEntranceDoor;

    int requiredItemsAmount = 4;
    
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void AddToScore(int scoreToAdd)
    {
        PlayerScore += scoreToAdd;
        SetScoreText();
    }

    public void AddToItemProgress()
    {
        itemProgress++;
        itemProgressText.text = "Items Found: " + itemProgress + "/" + requiredItemsAmount;
        if(itemProgress >= requiredItemsAmount)
        {
            Debug.Log("Logd");
            OpenBossEntrance();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
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
            bossEntranceDoor = GameObject.FindWithTag("BossEntranceDoor");
            levelExitDoor = GameObject.FindWithTag("LevelExitDoor");
            UpdateMuteText();
            SetScoreText();
            itemProgress = 0;
        }
    }

    void UpdateMuteText()
    {
        // If muted, say that the audio is muted. If not don't
        string mutedStatus = isMute ? "Muted" : "Active";
        muteText.text = "Mute: 'M'\nAudio: " + mutedStatus;
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + PlayerScore;
    }

    void Mute()
    {
        // Done using help from https://answers.unity.com/questions/829987/how-to-make-mute-button.html
        isMute = !isMute;
        AudioListener.volume = isMute ? 0 : 1;
        UpdateMuteText();
    }

    public void OpenLevelExit()
    {
        Destroy(levelExitDoor);
    }

    public void OpenBossEntrance()
    {
        Destroy(bossEntranceDoor);
    }

}
