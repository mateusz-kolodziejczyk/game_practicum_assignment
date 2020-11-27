using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapObjectType
{
    Empty,
    Wall,
    Player,
    RequiredItem,
    Enemy,
}

struct Level
{
    public MapObjectType[,] levelMap;
    public int numberOfItems;
    public int[] playerPosition;
    public List<(int x, int y)> enemyPositions;
}
public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject ground;
    [SerializeField]
    GameObject wall;
    [SerializeField]
    GameObject requiredItem;
    // This is for basic enemy types i.e. non bosses
    [SerializeField]
    List<GameObject> baseEnemyTypes;
    LocalNavMeshBuilder localNavMeshBuilder;

    // Store all levels
    List<Level> levels;


    [SerializeField]
    int NumberOfLevels = 3;

    void Awake()
    {
        levels = new List<Level>();
        localNavMeshBuilder = GameObject.FindWithTag("LocalNavMeshBuilder").GetComponent<LocalNavMeshBuilder>();
        CacheLevelInfo();
    }
    // Read each level from file on startup and cache the important information in arrays
    void CacheLevelInfo()
    {
        for(int i = 1; i <= NumberOfLevels; i++)
        {
            ReadFromImage("level2");
        }
    }

    void ReadFromImage(string imageName)
    {
        Texture2D image = (Texture2D)Resources.Load(imageName, typeof(Texture2D));

        var levelMap = new MapObjectType[image.width, image.height];
        localNavMeshBuilder.m_Size.x = image.width*11;
        localNavMeshBuilder.m_Size.z = image.height*11;
        int requiredItems = 0;
        int[] playerPosition = new int[2];
        // Use tuples for easy of retrieval
        var enemyPositions = new List<(int x, int y)>();

        Debug.Log("New Level");
        for (int x = 0; x < image.width; x++)
        {
            var s = "";
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
                    Debug.Log("Player added");
                    levelMap[x, y] = MapObjectType.Player;
                    playerPosition[0] = x;
                    playerPosition[1] = y;
                }
                else if(pixel == Color.blue)
                {
                    levelMap[x, y] = MapObjectType.RequiredItem;
                    requiredItems++;
                }
                else if(pixel == Color.red)
                {
                    // Have to instantiate enemies after waypoints, so log their position
                    enemyPositions.Add((x, y));
                }
                s += (int)levelMap[x, y];
            }
            Debug.Log(s);
        }
        levels.Add(new Level { levelMap = levelMap, numberOfItems = requiredItems, playerPosition = playerPosition, enemyPositions = enemyPositions });
        foreach(var (x,y) in enemyPositions)
        {
            Debug.Log(x + "," + y);
        }

        
    }

    // Returns the instantiated player object for further use
    public GameObject GenerateLevel(int levelNumber, GameObject player)
    {
        var currentLevel = levels[levelNumber];
        var ground = GameObject.FindWithTag("Ground");
        int mapWidth = currentLevel.levelMap.GetLength(0);
        int mapHeight = currentLevel.levelMap.GetLength(1);
        ground.transform.localScale = new Vector3(mapWidth * 1.5f, 1, mapHeight * 1.5f);

        GameObject playerObject = null;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                switch (currentLevel.levelMap[x,y])
                {
                    case MapObjectType.Empty:
                        break;
                    case MapObjectType.Wall:
                        InstantiateObject(wall, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.Player:
                        playerObject = InstantiateObject(player, x, y, mapWidth, mapHeight);
                        break;
                    case MapObjectType.RequiredItem:
                        InstantiateObject(requiredItem, x, y, mapWidth, mapHeight);
                        break;
                    default:
                        break;
                }
            }
        }
        SpawnEnemies(currentLevel);
        return playerObject;
    }

    GameObject InstantiateObject(GameObject objectToInstantiate, int x, int y, int mapWidth, int mapHeight)
    {
        return Instantiate(objectToInstantiate, new Vector3((mapWidth / 2 * 10) -  x * 10, 1.5f, (mapHeight / 2 * 10) - y * 10),
                            Quaternion.identity);
    }

    void SpawnEnemies(Level level)
    {
        foreach(var (x, y) in level.enemyPositions)
        {
            Debug.Log("enemy added");
            InstantiateObject(baseEnemyTypes[0], x, y, level.levelMap.GetLength(0), level.levelMap.GetLength(1));
        }
    }

}
