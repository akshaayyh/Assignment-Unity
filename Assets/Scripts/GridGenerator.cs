using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{


    // Grid settings
    public int gridWidth = 10;       // Width of the grid
    public int gridHeight = 10;      // Height of the grid
    public float spacing = 1f;       // Spacing between the cubes

    private Vector3[,] gridPositions;  // Array to store cube positions

    // Prefabs
    public GameObject cubePrefab;    // Reference to the cube prefab
    public GameObject playerPrefab;  // Reference to the player prefab
    public GameObject enemyPrefab;   // Reference to the enemy prefab
    void Start()
    {
        GenerateGrid();
        SpawnPlayer();

        SpawnEnemy();
    }


    // Method to generate the grid of cubes
    public void GenerateGrid()
    {
        gridPositions = new Vector3[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                GameObject cube= Instantiate(cubePrefab, position, Quaternion.identity);
                TileInfo tileInfo = cube.AddComponent<TileInfo>();
                tileInfo.x = x;
                tileInfo.y = y;

                gridPositions[x, y] = position; // Store the cube's position in gridPositions
            }
        }
       
    }

    // Method to spawn the player
    public void SpawnPlayer()
    {
        // Access the first element (0, 0) of the gridPositions array for the starting cube
        Vector3 playerStartPosition = gridPositions[0, 0];
        playerStartPosition.y = 1; // Adjust y-position if needed (e.g., placing player on top of the cube)
        Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
    }


    // Method to spawn the enemy
    public void SpawnEnemy()
    {
        Vector3 enemyStartPosition = gridPositions[gridWidth - 1, gridHeight - 1];
        enemyStartPosition.y = cubePrefab.transform.localScale.y; 
        Instantiate(enemyPrefab, enemyStartPosition, Quaternion.identity);
    }


    // Method to convert grid coordinates to world position
    public Vector3 GetWorldPosition(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            Vector3 position = gridPositions[x, y];
            position.y = cubePrefab.transform.localScale.y ; 
            return position;
        }
        else
        {
            Debug.LogError($"Invalid grid coordinates ({x}, {y}).");
            return Vector3.zero;
        }
    }

}
