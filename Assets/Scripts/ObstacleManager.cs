using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public ObstacleData obstacleData;
    public GameObject obstaclePrefab;
    public float gridSpacing = 1f;


    // Subscribe to event when script is enabled
    private void OnEnable()
    {
        obstacleData.OnObstacleDataChanged.AddListener(GenerateObstacles); // Subscribe to obstacle data change event
        GenerateObstacles();   // Generate obstacles initially

    }
    // Unsubscribe from event when script is disabled
    private void OnDisable()
    {
        obstacleData.OnObstacleDataChanged.RemoveListener(GenerateObstacles); // Unsubscribe from obstacle data change event
    }
    

    public void GenerateObstacles()   // Generate obstacles based on obstacleData
    {
        // Remove existing obstacles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < obstacleData.GridSizeX; x++)   // Generate new obstacles based on obstacleData grid
        {
            for (int y = 0; y < obstacleData.GridSizeY; y++)
            {
                if (obstacleData.GetObstacle(x, y))  // Check if there's an obstacle at (x, y)
                {
                    Vector3 position = new Vector3(x * gridSpacing, 1f, y * gridSpacing);
                    Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    // Check if a cell is blocked by an obstacle
    public bool IsCellBlocked(int x, int y)
    {
        if (x >= 0 && x < obstacleData.GridSizeX && y >= 0 && y < obstacleData.GridSizeY)
        {
            return obstacleData.GetObstacle(x, y);
        }
        return false; // Return false if coordinates are out of bounds
    }
}
