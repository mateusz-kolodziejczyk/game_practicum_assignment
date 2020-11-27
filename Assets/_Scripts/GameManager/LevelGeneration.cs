using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapObjectType
{
    Empty,
    Wall,
    Player,
    RequiredItem,
}

struct Level
{
    public MapObjectType[,] levelMap;
    public int numberOfItems;
    public int[] playerPosition;
}
public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject ground;
    [SerializeField]
    GameObject wall;
    [SerializeField]
    GameObject requiredItem;

    // Store all levels in a list
    List<Level> levels;

    int[] playerPosition = new int[2];

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
        for(int i = 1; i <= NumberOfLevels; i++)
        {
            ReadFromImage("level" + i);
        }
    }

    void ReadFromImage(string imageName)
    {
        Texture2D image = (Texture2D)Resources.Load("level1", typeof(Texture2D));

        var levelMap = new MapObjectType[image.width, image.height];
        int requiredItems = 0;
        int[] playerPosition = new int[2];

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
                    playerPosition[0] = x;
                    playerPosition[1] = y;
                }
                else if(pixel == Color.blue)
                {
                    levelMap[x, y] = MapObjectType.RequiredItem;
                    requiredItems++;
                }
            }
        }

        levels.Add(new Level { levelMap = levelMap, numberOfItems = requiredItems, playerPosition = playerPosition });
        
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
                        Instantiate(wall, new Vector3((mapWidth / 2 * 10) - x * 10, 1.5f, (mapHeight / 2 * 10) - y * 10),
                            Quaternion.identity);
                        break;
                    case MapObjectType.Player:
                        playerObject = Instantiate(player, new Vector3((mapWidth / 2 * 10) - currentLevel.playerPosition[0] * 10, 1.5f, (mapHeight / 2 * 10) - currentLevel.playerPosition[1] * 10),
                         Quaternion.identity);
                        break;
                    case MapObjectType.RequiredItem:
                        Instantiate(requiredItem, new Vector3((mapWidth / 2 * 10) - x * 10, 1.5f, (mapHeight / 2 * 10) - y * 10),
                            Quaternion.identity);
                        break;
                    default:
                        break;
                }
            }
        }
        return playerObject;
    }

}
