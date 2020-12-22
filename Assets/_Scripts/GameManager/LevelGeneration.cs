using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MapObjectType
{
    Empty,
    Wall,
    Player,
    RequiredItem,
    Enemy,
    Boss,
    BossEntry,
    LevelExit,
    ItemPickup,
    Lights,
}

struct Level
{
    public int id;
    public MapObjectType[,] levelMap;
    public int numberOfItems;
    public List<(int x, int y)> playerPositions;
    public List<(int x, int y)> enemyPositions;
    public Material wallMaterial;
}
public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject ground;
    [SerializeField]
    GameObject wall;
    [SerializeField]
    GameObject requiredItem;
    // Use the same object for both exit/entry, just change the tag when instantiated.
    [SerializeField]
    GameObject bossExitEntryDoor;
    [SerializeField]
    GameObject levelExitTrigger;
    [SerializeField]
    GameObject waypoint;
    [SerializeField]
    GameObject lights;


    // This is for basic enemy types i.e. non bosses
    [SerializeField]
    List<GameObject> baseEnemyTypes;
    [SerializeField]
    List<GameObject> bosses;
    [SerializeField]
    List<GameObject> itemPickups;
    LocalNavMeshBuilder localNavMeshBuilder;
    [SerializeField]
    List<Material> wallMaterials;

    // Store all levels
    List<Level> levels;


    [SerializeField]
    int NumberOfLevels = 3;

    void Awake()
    {
        levels = new List<Level>();
        CacheLevelInfo();
    }
    // Read each level from file on startup and cache the important information in arrays
    void CacheLevelInfo()
    {
        for (int i = 1; i <= NumberOfLevels; i++)
        {
            ReadFromImage("level" + i);
        }
    }

    void ReadFromImage(string imageName)
    {
        Texture2D image = (Texture2D)Resources.Load(imageName, typeof(Texture2D));

        var levelMap = new MapObjectType[image.width, image.height];

        int requiredItems = 0;

        var playerPositions = new List<(int x, int y)>();
        // Use tuples for easy of retrieval
        var enemyPositions = new List<(int x, int y)>();

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                var pixel = image.GetPixel(x, y);
                if (pixel == Color.white)
                {
                    levelMap[x, y] = MapObjectType.Empty;

                }
                else if (pixel == Color.black)
                {
                    levelMap[x, y] = MapObjectType.Wall;
                }
                else if (pixel == Color.green)
                {
                    levelMap[x, y] = MapObjectType.Player;
                    playerPositions.Add((x, y));
                }
                else if (pixel == Color.blue)
                {
                    levelMap[x, y] = MapObjectType.RequiredItem;
                    requiredItems++;
                }
                else if (pixel == Color.red)
                {
                    // Have to instantiate enemies after waypoints, so log their position
                    levelMap[x, y] = MapObjectType.Enemy;
                    enemyPositions.Add((x, y));
                }
                else if (pixel == Color.magenta)
                {
                    levelMap[x, y] = MapObjectType.Boss;
                }
                else if (pixel.r > 0.95 && pixel.g > 0.95 && pixel.b < 0.1)
                {
                    levelMap[x, y] = MapObjectType.BossEntry;
                }
                else if (pixel == Color.cyan)
                {
                    levelMap[x, y] = MapObjectType.LevelExit;
                }
                else if ((pixel.r >= 0.46 && pixel.r <= 0.54) && (pixel.g >= 0.46 && pixel.g <= 0.54) && (pixel.b >= 0.46 && pixel.b <= 0.54))
                {
                    levelMap[x, y] = MapObjectType.ItemPickup;
                }
                // Orange
                else if((pixel.r >= 0.9) && pixel.g >= 0.46 && pixel.g <= 0.54 && pixel.b <= 0.1)
                {
                    levelMap[x, y] = MapObjectType.Lights;
                }
            }
        }
        levels.Add(new Level 
        { 
            id = levels.Count+1, 
            levelMap = levelMap,
            numberOfItems = requiredItems,
            playerPositions = playerPositions, 
            enemyPositions = enemyPositions, 
            wallMaterial = wallMaterials[levels.Count] });
    }

    // Returns the instantiated player object for further use
    public GameObject GenerateLevel(int levelNumber, GameObject player)
    {
        var currentLevel = levels[levelNumber];
        var ground = GameObject.FindWithTag("Ground");
        int mapWidth = currentLevel.levelMap.GetLength(0);
        int mapHeight = currentLevel.levelMap.GetLength(1);
        ground.transform.localScale = new Vector3(mapWidth * 1.5f, 1, mapHeight * 1.5f);
        localNavMeshBuilder = GameObject.FindWithTag("LocalNavMeshBuilder").GetComponent<LocalNavMeshBuilder>();
        localNavMeshBuilder.m_Size.x = mapWidth * 11;
        localNavMeshBuilder.m_Size.z = mapHeight * 11;
        var bossPositions = new List<(int x, int y)>();
        GameObject playerObject = null;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                switch (currentLevel.levelMap[x, y])
                {
                    case MapObjectType.Empty:
                        break;
                    case MapObjectType.Wall:
                        var wallRenderer = wall.GetComponent<Renderer>();
                        wallRenderer.material = currentLevel.wallMaterial;
                        InstantiateObject(wall, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.Player:
                        break;
                    case MapObjectType.RequiredItem:
                        InstantiateObject(requiredItem, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.Boss:
                        bossPositions.Add((x, y));
                        break;
                    case MapObjectType.Enemy:
                        InstantiateObject(waypoint, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.BossEntry:
                        // Set the tag to entry
                        InstantiateObject(bossExitEntryDoor, x, y, mapWidth, mapHeight).tag = "BossEntranceDoor";
                        break;
                    case MapObjectType.LevelExit:
                        // Set the tag to exit
                        InstantiateObject(bossExitEntryDoor, x, y, mapWidth, mapHeight).tag = "LevelExitDoor";
                        InstantiateObject(levelExitTrigger, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.ItemPickup:
                        // This will make sure there are spots without either
                        int index = Random.Range(0, itemPickups.Count*2);
                        if (index < itemPickups.Count)
                        {
                            InstantiateObject(itemPickups[Random.Range(0, itemPickups.Count)], x, y, mapWidth, mapHeight);
                        }
                        break;
                    case MapObjectType.Lights:
                        InstantiateObject(lights, x, y, mapWidth, mapHeight, 6.8f);
                        break;
                    default:
                        break;
                }
            }
        }
        int startingPositionIndex = Random.Range(0, currentLevel.playerPositions.Count);
        var startingPosition = currentLevel.playerPositions[startingPositionIndex];
        playerObject = InstantiateObject(player, startingPosition.x, startingPosition.y, mapWidth, mapHeight);
        // Instantiate boss and enemies after the player has been instantiated
        GetComponent<GameManagement>().RequiredItemsAmount = currentLevel.numberOfItems;
        SpawnEnemies(currentLevel);
        // The boss gets stuck in a wall unless the navmeshagent is disabled and the boss is manually placed back to its position.
        //InstantiateObject(bosses[0], bossPosition.x, bossPosition.y, mapWidth, mapHeight);
        foreach(var bossPosition in bossPositions)
        {
            var bossCoroutine = BossNavigationPosition(bosses[currentLevel.id-1], bossPosition, mapWidth, mapHeight);
            StartCoroutine(bossCoroutine);
        }
        return playerObject;
    }

    IEnumerator BossNavigationPosition(GameObject boss, (int x, int y) bossPosition, int mapWidth, int mapHeight)
    {
        boss = InstantiateObject(boss, bossPosition.x, bossPosition.y, mapWidth, mapHeight);
        var bossAgent = boss.GetComponent<NavMeshAgent>();
        bossAgent.enabled = false;
        boss.transform.position = new Vector3((mapWidth / 2 * 10) - bossPosition.x * 10, 1.5f, (mapHeight / 2 * 10) - bossPosition.y * 10);
        yield return new WaitForSeconds(0.5f);
        bossAgent.enabled = true;
    }

    GameObject InstantiateObject(GameObject objectToInstantiate, int x, int y, int mapWidth, int mapHeight)
    {
        return Instantiate(objectToInstantiate, new Vector3((mapWidth / 2 * 10) - x * 10, 1.5f, (mapHeight / 2 * 10) - y * 10),
                            Quaternion.identity);
    }
    GameObject InstantiateObject(GameObject objectToInstantiate, int x, int y, int mapWidth, int mapHeight, float height)
    {
        return Instantiate(objectToInstantiate, new Vector3((mapWidth / 2 * 10) - x * 10, height, (mapHeight / 2 * 10) - y * 10),
                            Quaternion.identity);
    }
    void SpawnEnemies(Level level)
    {
        foreach (var (x, y) in level.enemyPositions)
        {
            // First level only had melee enemies
            if (level.id < 2)
            {
                InstantiateObject(baseEnemyTypes[0], x, y, level.levelMap.GetLength(0), level.levelMap.GetLength(1));
            }
            else
            {
                int enemyToInstantiate = Random.Range(0, baseEnemyTypes.Count);
                InstantiateObject(baseEnemyTypes[enemyToInstantiate], x, y, level.levelMap.GetLength(0), level.levelMap.GetLength(1));
            }
        }
    }

}
