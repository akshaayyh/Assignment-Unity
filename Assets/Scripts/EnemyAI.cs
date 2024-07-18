using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the enemy
    public float stoppingDistance = 0.1f; // Distance at which enemy stops moving
    public float checkPlayerMoveInterval = 0.5f; // Time interval to check if the player has moved

    private Transform playerTransform; // Reference to the player transform
    private GridGenerator gridGenerator; // Reference to the grid generator script
    private ObstacleManager obstacleManager; // Reference to obstacle manager script
    private AStarPathfinding pathfinder; // Reference to A* pathfinding script
    private PlayerMovement playerMovement; // Reference to the player movement script

    private List<Vector3> currentPath = new List<Vector3>(); // Current path to follow
    private int currentPathIndex; // Index of the current waypoint in the path
    private bool isMoving; // Flag to check if the enemy is currently moving
    private Vector3 lastPlayerPosition; // Last known position of the player

    public void InitializeAI()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gridGenerator = FindObjectOfType<GridGenerator>();
        obstacleManager = FindObjectOfType<ObstacleManager>();
        pathfinder = GetComponent<AStarPathfinding>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        lastPlayerPosition = playerTransform.position;
        StartCoroutine(CheckPlayerMovement());
    }

    public void MoveAI()
    {
        if (!isMoving && currentPath.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator CheckPlayerMovement()
    {
        while (true)
        {
            if (playerTransform.position != lastPlayerPosition)
            {
                Vector3 targetPosition = GetAdjacentTileToPlayer(playerTransform.position);
                currentPath = pathfinder.FindPath(transform.position, targetPosition);
                currentPathIndex = 0;
                lastPlayerPosition = playerTransform.position;
            }
            yield return new WaitForSeconds(checkPlayerMoveInterval);
        }
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        while (currentPathIndex < currentPath.Count)
        {
            Vector3 target = currentPath[currentPathIndex];

            while (Vector3.Distance(transform.position, target) > stoppingDistance)
            {
                target.y = gridGenerator.cubePrefab.transform.localScale.y / 2f; // Keep y position consistent
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;
        }

        isMoving = false;
    }

    private Vector3 GetAdjacentTileToPlayer(Vector3 playerPosition)
    {
        Vector2Int playerCoord = WorldToGridCoordinates(playerPosition);
        List<Vector2Int> adjacentCoords = new List<Vector2Int>
        {
            new Vector2Int(playerCoord.x + 1, playerCoord.y),
            new Vector2Int(playerCoord.x - 1, playerCoord.y),
            new Vector2Int(playerCoord.x, playerCoord.y + 1),
            new Vector2Int(playerCoord.x, playerCoord.y - 1)
        };

        adjacentCoords.RemoveAll(coord => coord.x < 0 || coord.x >= gridGenerator.gridWidth || coord.y < 0 || coord.y >= gridGenerator.gridHeight || obstacleManager.IsCellBlocked(coord.x, coord.y)); 

        if (adjacentCoords.Count > 0)
        {
            Vector2Int chosenCoord = adjacentCoords[Random.Range(0, adjacentCoords.Count)];
            return GridToWorldCoordinates(chosenCoord);
        }

        return playerPosition; // Fallback to player's position if no adjacent tiles are valid
    }

    private Vector2Int WorldToGridCoordinates(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.spacing);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.spacing); // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorldCoordinates(Vector2Int gridCoord)
    {
        float x = gridCoord.x * gridGenerator.spacing;
        float z = gridCoord.y * gridGenerator.spacing; // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector3(x, gridGenerator.cubePrefab.transform.localScale.y / 2f, z);
    }

    public bool IsOccupied(int x, int y)
    {
        Vector2Int enemyCoord = WorldToGridCoordinates(transform.position);
        return enemyCoord.x == x && enemyCoord.y == y;
    }
}
