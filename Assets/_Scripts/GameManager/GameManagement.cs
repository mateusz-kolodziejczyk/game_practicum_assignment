using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ECM.Controllers;
using System.Linq;
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
    GameObject ingameMenuObject;
    [SerializeField]
    GameObject player;
    // UI
    Text itemProgressText;
    Text scoreText;
    Text muteText;
    Text livesText;
    Text timeText;
    GameObject exitConfirmText;

    GameObject ingameMenu;
    int itemProgress;
    bool isMute = false;
    bool isPaused = false;

    // The unmuted sound volume will be stored here
    float soundVolume = 1;

    GameObject levelExitDoor;
    GameObject bossEntranceDoor;


    List<Enemy> enemiesOnLevel = new List<Enemy>();

    // Player Lives
    public int MaxLives { get; set; } = 3;
    public int CurrentLives { get; set; }
    public float MaxHealth { get; set; } = 100;



    // Weapons
    // Weapons Inventory contains all weapons, but is restricted by the unlocked weapon id's list
    public Dictionary<int, Weapon> WeaponsInventory { get; set; } = new Dictionary<int, Weapon>();
    private List<int> UnlockedWeaponIDs { get; set; } = new List<int>();
    public int ActiveWeaponID { get; set; } = 1;
    WeaponPanel weaponPanel;

    public int RequiredItemsAmount { get; set; } = 3;
    // Difficulty 
    Dropdown difficultySelection;
    public DifficultyLevel CurrentDifficulty { get; set; } = DifficultyLevel.Hard;

    // This dictionary holds the multipliers for attack speed/damage/health
    Dictionary<DifficultyLevel, float> difficultyMultipliers = new Dictionary<DifficultyLevel, float>();

    BaseFirstPersonController playerController;


    IEnumerator showWeaponPanel;
    IEnumerator quitGame;

    bool countingKillStreak = false;
    IEnumerator killStreak;
    int killStreakCount = 0;

    IEnumerator takingDamage;
    bool countingDamageTaken = false;
    float damageTaken = 0;

    bool quitConfirm = false;

    float levelTimer;

    float dynamicDifficultyMultiplier = 1;

    void Awake()
    {
        levelGeneration = GetComponent<LevelGeneration>();

        // start the game off with only 1 weapon index equal to the single shooter one
        UnlockedWeaponIDs.Add(1);
        UnlockedWeaponIDs.Add(2);
        UnlockedWeaponIDs.Add(3);
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
        // Only update level timer if its inside level
        if (SceneManager.GetActiveScene().buildIndex > 1 && SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            levelTimer += Time.deltaTime;
            updateTimeText();
        }
        if (Input.GetKeyDown("m"))
        {
            Mute();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!quitConfirm)
            {
                quitGame = QuitGame(3);
                StartCoroutine(quitGame);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    private IEnumerator QuitGame(float maxTime)
    {
        quitConfirm = true;
        float timePassed = 0;
        exitConfirmText.SetActive(true);
        while (timePassed < maxTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        exitConfirmText.SetActive(false);
        quitConfirm = false;
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
            enemy.MaxHealth *= difficultyMultipliers[CurrentDifficulty] * dynamicDifficultyMultiplier;
            enemy.Health *= difficultyMultipliers[CurrentDifficulty] * dynamicDifficultyMultiplier;
            enemy.Damage *= difficultyMultipliers[CurrentDifficulty] * dynamicDifficultyMultiplier;
            // Lower = more dangeorus so divide 1 by the number
            enemy.TimeBetweenAttacks *= 1f / difficultyMultipliers[CurrentDifficulty];
        }
    }
    // This function is specifically for adjusting abilities after the level spawned
    private void AdjustEnemyAttributes(float difficultyMultiplier, bool divide, List<Enemy> enemies)
    {

        // Go through each enemy and change the attributes
        foreach (Enemy enemy in enemies)
        {
            // Divide the values(used for going back after killstreak)
            if (divide)
            {
                // Multiply the variables by the value of the multipliers gotten using the key
                enemy.MaxHealth /= difficultyMultiplier;
                enemy.Health /= difficultyMultiplier;
                enemy.Damage /= difficultyMultiplier;
                // Lower = more dangeorus so divide 1 by the number
                enemy.TimeBetweenAttacks /= (1f / difficultyMultiplier);
            }
            else
            {
                // Multiply the variables by the value of the multipliers gotten using the key
                enemy.MaxHealth *= difficultyMultiplier;
                enemy.Health *= difficultyMultiplier;
                enemy.Damage *= difficultyMultiplier;
                // Lower = more dangeorus so divide 1 by the number
                enemy.TimeBetweenAttacks *= (1f / difficultyMultiplier);
            }
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
        SetItemProgressText();
        if (itemProgress >= RequiredItemsAmount)
        {
            Debug.Log("Logd");
            UnlockWeapon(UnlockedWeaponIDs.Last() + 1);
            OpenBossEntrance();
        }
    }

    public void SetItemProgressText()
    {
        itemProgressText.text = "Weapon Parts Found: " + itemProgress + "/" + RequiredItemsAmount;
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
        if (showWeaponPanel != null)
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

    // Kill Streak
    // This manages the running of coroutines for killstreak.
    public void AddToKillStreak()
    {
        killStreakCount++;
        if (!countingKillStreak)
        {
            killStreak = StartKillStreak();
            StartCoroutine(killStreak);
        }
    }

    public IEnumerator StartKillStreak()
    {
        countingKillStreak = true;
        // Wait 10 seconds and keep counting new kills.
        yield return new WaitForSeconds(10);
        var killStreakDifficultyAdjustment = KillStreakDifficultyManagement(killStreakCount);
        StartCoroutine(killStreakDifficultyAdjustment);
        killStreakCount = 0;
        countingKillStreak = false;
    }

    // Kill streak only last a certain amount of time and they can overlap.
    private IEnumerator KillStreakDifficultyManagement(int killStreakCount)
    {

        var enemies = GetCurrentEnemies();
        var difficultyMultiplier = CalculateKillStreakDifficultyMultiplier(killStreakCount);
        Debug.Log(difficultyMultiplier);
        AdjustEnemyAttributes(difficultyMultiplier, false, enemies);

        yield return new WaitForSeconds(20);

        enemies = GetCurrentEnemies();
        AdjustEnemyAttributes(difficultyMultiplier, true, enemies);

    }

    // Resets dynamic difficulty adjustment coroutines so they stop when a new level is launched.
    void ResetDynamicDifficulty()
    {
        StopAllCoroutines();
    }

    public void AddToDamageCounter(float damage)
    {
        damageTaken += damage;
        if (!countingDamageTaken)
        {
            takingDamage = StartDamageCounter();
            StartCoroutine(takingDamage);
        }
    }

    public void BoostDamage(float damageMultiplier)
    {
        foreach(var weapon in WeaponsInventory.Values)
        {
            weapon.BoostDamage(damageMultiplier);
        }
    }

    IEnumerator StartDamageCounter()
    {
        countingDamageTaken = true;
        yield return new WaitForSeconds(5);
        StartCoroutine(DamageTakenDifficultyAdjustment(damageTaken));
        damageTaken = 0;
        countingDamageTaken = false;
    }

    IEnumerator DamageTakenDifficultyAdjustment(float damageTaken)
    {
        var enemies = GetCurrentEnemies();
        var difficultyMultiplier = CalculateDamageTakenDifficultyMultiplier(damageTaken);
        AdjustEnemyAttributes(difficultyMultiplier, false, enemies);

        yield return new WaitForSeconds(20);

        enemies = GetCurrentEnemies();
        AdjustEnemyAttributes(difficultyMultiplier, true, enemies);
    }


    private List<Enemy> GetCurrentEnemies()
    {
        // Get current enemies
        var enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        var enemies = new List<Enemy>();
        foreach (var enemyObject in enemyObjects)
        {
            enemies.Add(enemyObject.GetComponent<Enemy>());
        }
        return enemies;
    }

    private float CalculateKillStreakDifficultyMultiplier(int enemiesKilled)
    {
        // Use an exponent, the more enemies you kill the harder it becomes. One enmy kill streak is very inconsequential, 20 enmies would double their damage/health
        return 1 * Mathf.Pow(1.02f, enemiesKilled);
    }

    private float CalculateDamageTakenDifficultyMultiplier(float damageTaken)
    {
        // The max decrease in health/damage is 0.7f given where damagetaken/maxhealth = 1
        return 1 * Mathf.Pow(0.7f, damageTaken / MaxHealth);
    }

    // Methods for loading scenes
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadNextLevel()
    {
        // Check the timer
        DifficultyOnTimer();
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
                ResetDynamicDifficulty();
                levelTimer = 0;
                Instantiate(HUD);
                ingameMenu = Instantiate(ingameMenuObject);
                ingameMenu.SetActive(false);
                // Generate level by number currently 2 non game scenes
                playerController = levelGeneration.GenerateLevel(scene.buildIndex - 2, player).GetComponent<BaseFirstPersonController>();

                SetupWeaponInventory();
                // Add all enemies in the level into an array
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                var bosses = GameObject.FindGameObjectsWithTag("Boss");
                foreach (var enemyGameObject in enemies)
                {
                    enemiesOnLevel.Add(enemyGameObject.GetComponent<Enemy>());
                }
                foreach(var bossGameObject in bosses)
                {
                    enemiesOnLevel.Add(bossGameObject.GetComponent<Enemy>());
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
                SetItemProgressText();
                updateTimeText();

                //Find the active weapon, set it to active
                var weaponManagement = GameObject.FindWithTag("WeaponManagement").GetComponent<WeaponManagement>();
                weaponManagement.ActiveWeapon = WeaponsInventory[ActiveWeaponID];

            }
            else if (scene.buildIndex == 0 || scene.buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                levelTimer = 0;
                MaxHealth = 100;
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
        timeText = GameObject.FindWithTag("TimeText").GetComponent<Text>();
        exitConfirmText = GameObject.FindWithTag("ExitConfirmText");
        exitConfirmText.SetActive(false);
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

    void updateTimeText()
    {
        int minutes = (int)levelTimer / 60;
        int seconds = (int)levelTimer - minutes * 60;
        string secondsAsString = seconds >= 10 ? seconds.ToString() : "0" + seconds;
        timeText.text = "Time: " + minutes + ":" + secondsAsString;
    }

    // Sound
    public void Mute()
    {
        // Done using help from https://answers.unity.com/questions/829987/how-to-make-mute-button.html
        isMute = !isMute;
        AudioListener.volume = isMute ? 0 : soundVolume;
        UpdateMuteText();
    }
    public void Mute(bool toMute)
    {
        isMute = toMute;
        AudioListener.volume = isMute ? 0 : soundVolume;
        if (muteText != null)
        {
            UpdateMuteText();
        }
    }
    public void Changevolume(float newVolume)
    {
        soundVolume = newVolume;
        AudioListener.volume = isMute ? 0 : soundVolume;
    }

    // Done using help from  https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
    public void PauseGame()
    {
        isPaused = !isPaused;
        playerController.Pause(isPaused);
        // Set the timescale to 0 if paused, 1 if not
        Time.timeScale = isPaused ? 0 : 1;
        if (isPaused)
        {
            ingameMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ingameMenu.SetActive(false);
        }
    }

    public void OpenLevelExit()
    {
        Destroy(levelExitDoor);
    }

    public void OpenBossEntrance()
    {
        Destroy(bossEntranceDoor);
    }

    // Change difficulty dynamically based on how fast the level was finished
    private void DifficultyOnTimer()
    {
        // Value is a placeholder, will increase/decrease difficulty as a ratio of averagetime to actual time
        dynamicDifficultyMultiplier = 180 / levelTimer;
        if (dynamicDifficultyMultiplier > 1.25f)
        {
            dynamicDifficultyMultiplier = 1.25f;
        }
        else if (dynamicDifficultyMultiplier < 0.8f)
        {
            dynamicDifficultyMultiplier = 0.8f;
        }
    }

}
