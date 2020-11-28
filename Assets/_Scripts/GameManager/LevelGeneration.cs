﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapObjectType
{
    Empty,
    Wall,
    Player,
    RequiredItem,
    Enemy,
    Boss,
    BossEntry,
    LevelExit
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
    // Use the same object for both exit/entry, just change the tag when instantiated.
    [SerializeField]
    GameObject bossExitEntryDoor;
    [SerializeField]
    GameObject levelExitTrigger;


    // This is for basic enemy types i.e. non bosses
    [SerializeField]
    List<GameObject> baseEnemyTypes;
    [SerializeField]
    List<GameObject> bosses;
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

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                var pixel = image.GetPixel(x, y);
                if (pixel == Color.white || pixel == Color.gray)
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
                else if(pixel  == Color.magenta)
                {
                    levelMap[x, y] = MapObjectType.Boss;
                }
                else if(pixel.r > 0.95 && pixel.g > 0.95 && pixel.b < 0.1)
                {
                    Debug.Log("Yeet");
                    levelMap[x, y] = MapObjectType.BossEntry;
                }
                else if(pixel == Color.cyan)
                {
                    levelMap[x, y] = MapObjectType.LevelExit;
                }
            }
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

        (int x, int y) bossPosition = (0,0);
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
                    case MapObjectType.Boss:
                        bossPosition = (x, y);
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
                    default:
                        break;
                }
            }
        }
        // Instantiate boss and enemies after the player has been instantiated
        InstantiateObject(bosses[0], bossPosition.x, bossPosition.y, mapWidth, mapHeight);
        GetComponent<GameManagement>().RequiredItemsAmount = currentLevel.numberOfItems;
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
