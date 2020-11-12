using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}
public class GameManagement : MonoBehaviour
{
    public static GameManagement instance;
    public int PlayerScore { get; set; } = 0;
    // UI
    Text itemProgressText;
    Text scoreText;
    Text muteText;

    int itemProgress;
    bool isMute = false;

    GameObject levelExitDoor;
    GameObject bossEntranceDoor;


    List<Enemy> enemiesOnLevel = new List<Enemy>();



    // Weapons
    public Dictionary<int, Weapon> WeaponsInventory { get; set; }
    public int ActiveWeaponID { get; set; }
    WeaponPanel weaponPanel;

    // Goals
    int requiredItemsAmount = 4;
    // Difficulty 
    Dropdown difficultySelection;
    public DifficultyLevel CurrentDifficulty { get; set; } = DifficultyLevel.Hard;

    // This dictionary holds the multipliers for attack speed/damage/health
    Dictionary<DifficultyLevel, float> difficultyMultipliers = new Dictionary<DifficultyLevel, float>();

    void Awake()
    {

        // Adding the multipiers
        difficultyMultipliers.Add(DifficultyLevel.Easy, 0.7f);
        difficultyMultipliers.Add(DifficultyLevel.Medium, 1f);
        difficultyMultipliers.Add(DifficultyLevel.Hard, 1.5f);
            
        // Setup the weapon inventory
        WeaponsInventory = new Dictionary<int, Weapon>();
        var weapons = GameObject.FindGameObjectsWithTag("Weapon");

        // Adding all the weapons to the inventory
        foreach (var weapon in weapons)
        {
            var weaponScript = weapon.GetComponent<Weapon>();
            WeaponsInventory.Add(weaponScript.WeaponID, weaponScript);
        }
        ActiveWeaponID = 1;

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
    }

    // Methods
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

    public void AddWeaponToInventory(Weapon weapon)
    {
        WeaponsInventory.Add(weapon.WeaponID, weapon);
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
        if (ActiveWeaponID > WeaponsInventory.Count)
        {
            ActiveWeaponID = 1;
        }
        else if (ActiveWeaponID < 1)
        {
            ActiveWeaponID = WeaponsInventory.Count;
        }

        weaponPanel.switchWeaponHighlight(ActiveWeaponID);
    }

    // Adjust enemy values like health/damage/attack speed here
    private void AdjustEnemyAttributes()
    {
        // Go through each enemy and change the attributes
        foreach(Enemy enemy in enemiesOnLevel)
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
        if (WeaponsInventory.Count >= weaponIndex)
        {
            ActiveWeaponID = weaponIndex;
            weaponPanel.switchWeaponHighlight(ActiveWeaponID);
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
            // Only look for these things in non menu indexes
            if (scene.buildIndex > 0)
            {
                // Add all enemies in the level into an array
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemyGameObject in enemies)
                {
                    enemiesOnLevel.Add(enemyGameObject.GetComponent<Enemy>());
                }
                // Adjust enemy values
                AdjustEnemyAttributes();

                // Setup ui references
                weaponPanel = GameObject.FindWithTag("WeaponPanel").GetComponent<WeaponPanel>();
                itemProgressText = GameObject.FindWithTag("ItemProgressText").GetComponent<Text>();
                scoreText = GameObject.FindWithTag("Score Text").GetComponent<Text>();
                muteText = GameObject.FindWithTag("MuteText").GetComponent<Text>();
                // Two doors unlocked by progressing through the level
                bossEntranceDoor = GameObject.FindWithTag("BossEntranceDoor");
                levelExitDoor = GameObject.FindWithTag("LevelExitDoor");
                // Set the text again on the level loaidng in
                UpdateMuteText();
                SetScoreText();
                itemProgress = 0;
               
            }
            else if (scene.buildIndex == 0)
            {
                // unlock the cursor on loading the menu(if you're coming back from playing)
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Get the difficulty dropdown
                difficultySelection = GameObject.FindWithTag("DifficultySelection").GetComponent<Dropdown>();
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
