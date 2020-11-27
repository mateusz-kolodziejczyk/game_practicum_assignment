﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ECM.Controllers;
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}
// This class handles UI as well as any information that needs to be kept across levels.
public class GameManagement : MonoBehaviour
{
    public static GameManagement instance;
    public int PlayerScore { get; set; } = 0;

    LevelGeneration levelGeneration;
    [SerializeField]
    GameObject HUD;
    [SerializeField]
    GameObject player;
    // UI
    Text itemProgressText;
    Text scoreText;
    Text muteText;
    Text livesText;

    int itemProgress;
    bool isMute = false;
    bool isPaused = false;

    GameObject levelExitDoor;
    GameObject bossEntranceDoor;


    List<Enemy> enemiesOnLevel = new List<Enemy>();

    // Player Lives
    public int MaxLives { get; set; } = 3;
    public int CurrentLives { get; set; }



    // Weapons
    // Weapons Inventory contains all weapons, but is restricted by the unlocked weapon id's list
    public Dictionary<int, Weapon> WeaponsInventory { get; set; } = new Dictionary<int, Weapon>();
    private List<int> UnlockedWeaponIDs { get; set; } = new List<int>();
    public int ActiveWeaponID { get; set; } = 1;
    WeaponPanel weaponPanel;

    // Goals
    int requiredItemsAmount = 3;
    // Difficulty 
    Dropdown difficultySelection;
    public DifficultyLevel CurrentDifficulty { get; set; } = DifficultyLevel.Hard;

    // This dictionary holds the multipliers for attack speed/damage/health
    Dictionary<DifficultyLevel, float> difficultyMultipliers = new Dictionary<DifficultyLevel, float>();

    BaseFirstPersonController playerController;

    //WeaponManagement weaponManagement;

    IEnumerator showWeaponPanel;


    void Awake()
    {
        levelGeneration = GetComponent<LevelGeneration>();

        // start the game off with only 1 weapon index equal to the single shooter one
        UnlockedWeaponIDs.Add(1);
        CurrentLives = MaxLives;
        // Only for testing if starting inside a level


        // Adding the multipiers
        difficultyMultipliers.Add(DifficultyLevel.Easy, 0.7f);
        difficultyMultipliers.Add(DifficultyLevel.Medium, 1f);
        difficultyMultipliers.Add(DifficultyLevel.Hard, 1.5f);

        // Getting difficulty selection and adding a listener
        // Only do if on the start screen(This is for testing, game will always start on menu)
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // Get the difficulty dropdown
            difficultySelection = GameObject.FindWithTag("DifficultySelection").GetComponent<Dropdown>();
            // Using code from https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Dropdown-onValueChanged.html
            difficultySelection.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(difficultySelection);
            });
        }

        // Only have one instance of game management running
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    // Methods
    // Difficulty
    private void DropdownValueChanged(Dropdown change)
    {
        // Cast the int into the difficulty level enum
        switch ((DifficultyLevel)change.value)
        {
            case DifficultyLevel.Easy:
                CurrentDifficulty = DifficultyLevel.Easy;
                break;
            case DifficultyLevel.Medium:
                CurrentDifficulty = DifficultyLevel.Medium;
                break;
            case DifficultyLevel.Hard:
                CurrentDifficulty = DifficultyLevel.Hard;
                break;
            default:
                break;
        }
        Debug.Log(CurrentDifficulty);
    }
    public void ChangeDifficultyLevel(DifficultyLevel difficulty)
    {
        CurrentDifficulty = difficulty;
    }

    //Weapons
    public void AddWeaponToInventory(Weapon weapon)
    {
        WeaponsInventory.Add(weapon.WeaponID, weapon);
    }

    // Player Lives
    public void ChangeLives(bool increase)
    {
        if (increase)
        {
            // Only increase max lives if less than max lives
            if (CurrentLives < MaxLives)
            {
                CurrentLives++;
            }
        }
        else
        {
            CurrentLives--;
        }

        SetLivesText();
    }

    // If bool is true increase id by 1, otherwise decreease by 1
    public void ChangeWeaponScrolling(bool changingUp)
    {
        if (changingUp)
        {
            ActiveWeaponID += 1;
        }
        else
        {
            ActiveWeaponID -= 1;
        }
        // Wrap the id around.
        if (ActiveWeaponID > UnlockedWeaponIDs.Count)
        {
            ActiveWeaponID = 1;
        }
        else if (ActiveWeaponID < 1)
        {
            ActiveWeaponID = UnlockedWeaponIDs.Count;
        }

        weaponPanel.switchWeaponHighlight(ActiveWeaponID);

        StartShowPanelCoroutine();
    }

    // Adjust enemy values like health/damage/attack speed here
    private void AdjustEnemyAttributes()
    {
        // Go through each enemy and change the attributes
        foreach (Enemy enemy in enemiesOnLevel)
        {
            // Multiply the variables by the value of the multipliers gotten using the key
            enemy.Health *= difficultyMultipliers[CurrentDifficulty];
            enemy.Damage *= difficultyMultipliers[CurrentDifficulty];
            // Lower = more dangeorus so divide 1 by the number
            enemy.TimeBetweenAttacks *= 1f / difficultyMultipliers[CurrentDifficulty];
        }
    }


    public bool ChangeWeaponByIndex(int weaponIndex)
    {
        // If there are more weapon indexes than ewapons in the inventory, do not do anything
        if (UnlockedWeaponIDs.Count >= weaponIndex)
        {
            ActiveWeaponID = weaponIndex;
            weaponPanel.switchWeaponHighlight(ActiveWeaponID);

            StartShowPanelCoroutine();
            return true;
        }
        else
        {
            return false;
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
        if (itemProgress >= requiredItemsAmount)
        {
            Debug.Log("Logd");
            UnlockWeapon(ActiveWeaponID+1);
            OpenBossEntrance();
        }
    }

    void UnlockWeapon(int weaponID)
    {
        // ONly run if the weapon isnt already unlocked, and the weapons inventory contains the id
        if (!UnlockedWeaponIDs.Contains(weaponID) && WeaponsInventory.ContainsKey(weaponID))
        {
            UnlockedWeaponIDs.Add(weaponID);
            weaponPanel.updatePanels(UnlockedWeaponIDs);
            // Show panel to indicate there is a new weapon
            StartShowPanelCoroutine();
        }
    }

    void StartShowPanelCoroutine()
    {
        if(showWeaponPanel != null)
        {
            StopCoroutine(showWeaponPanel);
        }
        showWeaponPanel = ShowWeaponPanel();
        StartCoroutine(showWeaponPanel);
    }

    IEnumerator ShowWeaponPanel()
    {
        weaponPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        weaponPanel.gameObject.SetActive(false);
    }

    // Methods for loading scenes
    public void StartGame()
    {
        SceneManager.LoadScene(2);
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

    public void LoadInstructionScreen()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGameOverScreen()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Using help from https://answers.unity.com/questions/1580211/when-an-object-is-dontdestroyonload-where-can-i-pu.html
        if (scene.isLoaded)
        {
            // Only look for these things in non menu indexes and non game over(game over is always the last index)
            if (scene.buildIndex > 1 && scene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
               
                Instantiate(HUD);
                // Generate level by number currently 2 non game scenes
                playerController = levelGeneration.GenerateLevel(scene.buildIndex-2, player).GetComponent<BaseFirstPersonController>(); 

                SetupWeaponInventory();
                // Add all enemies in the level into an array
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemyGameObject in enemies)
                {
                    enemiesOnLevel.Add(enemyGameObject.GetComponent<Enemy>());
                }
                // Adjust enemy values
                AdjustEnemyAttributes();

                SetupUI();
                weaponPanel.switchWeaponHighlight(ActiveWeaponID);
                weaponPanel.updatePanels(UnlockedWeaponIDs);
                weaponPanel.gameObject.SetActive(false);
                // Two doors unlocked by progressing through the level
                bossEntranceDoor = GameObject.FindWithTag("BossEntranceDoor");
                levelExitDoor = GameObject.FindWithTag("LevelExitDoor");
                // Set the text again on the level loaidng in
                UpdateMuteText();
                SetScoreText();
                SetLivesText();
                itemProgress = 0;

                //Find the active weapon, set it to active
                var weaponManagement = GameObject.FindWithTag("WeaponManagement").GetComponent<WeaponManagement>();
                weaponManagement.ActiveWeapon = WeaponsInventory[ActiveWeaponID];

            }
            else if (scene.buildIndex == 0 || scene.buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                // Clear all weapons
                WeaponsInventory.Clear();
                UnlockedWeaponIDs.Clear();
                UnlockedWeaponIDs.Add(1);
                // unlock the cursor on loading the menu(if you're coming back from playing)
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (scene.buildIndex == 0)
                {
                    // Get the difficulty dropdown
                    difficultySelection = GameObject.FindWithTag("DifficultySelection").GetComponent<Dropdown>();
                }
            }

        }
    }

    void SetupUI()
    {
        // Setup ui references
        weaponPanel = GameObject.FindWithTag("WeaponPanel").GetComponent<WeaponPanel>();
        itemProgressText = GameObject.FindWithTag("ItemProgressText").GetComponent<Text>();
        scoreText = GameObject.FindWithTag("Score Text").GetComponent<Text>();
        muteText = GameObject.FindWithTag("MuteText").GetComponent<Text>();
        livesText = GameObject.FindWithTag("LivesText").GetComponent<Text>();
    }

    public void IncreaseAmmo(float multiplier)
    {
        var currentWeapon = WeaponsInventory[ActiveWeaponID];
        currentWeapon.CurrentAmmo += (int)(currentWeapon.CurrentAmmo * multiplier);
        currentWeapon.setAmmoText();
    }
    void SetupWeaponInventory()
    {
        // If not empty
        if (WeaponsInventory.Count > 0)
        {
            WeaponsInventory.Clear();
        }
        // Add all weapons
        var weapons = GameObject.FindGameObjectsWithTag("Weapon");

        // Adding all the weapons to the inventory
        foreach (var weapon in weapons)
        {
            var weaponScript = weapon.GetComponent<Weapon>();
            WeaponsInventory.Add(weaponScript.WeaponID, weaponScript);
            // Set all weapons besides the currently equipped one to inactive
            if (weaponScript.WeaponID == ActiveWeaponID)
            {
                // Set ammo to the first weapon
                weapon.SetActive(true);
                weaponScript.setAmmoText();
            }
            else
            {
                weapon.SetActive(false);
            }
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

    void SetLivesText()
    {
        livesText.text = "Lives: " + CurrentLives;
    }

    void Mute()
    {
        // Done using help from https://answers.unity.com/questions/829987/how-to-make-mute-button.html
        isMute = !isMute;
        AudioListener.volume = isMute ? 0 : 1;
        UpdateMuteText();
    }

    // Done using help from  https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
    void PauseGame()
    {
        isPaused = !isPaused;
        playerController.Pause(isPaused);
        // Set the timescale to 0 if paused, 1 if not
        Time.timeScale = isPaused ? 0 : 1;


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