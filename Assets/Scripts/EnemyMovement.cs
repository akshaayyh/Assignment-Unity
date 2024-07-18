using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the enemy
    public float stoppingDistance = 0.1f; // Distance at which enemy stops moving
   

    private Transform playerTransform; // Reference to the player transform
    private GridGenerator gridGenerator; // Reference to the grid generator script
    private ObstacleManager obstacleManager; // Reference to obstacle manager script
    private AStarPathfinding pathfinder; // Reference to A* pathfinding script

    private List<Vector3> currentPath = new List<Vector3>(); // Current path to follow
    private int currentPathIndex; // Index of the current waypoint in the path
    private bool isMoving; // Flag to check if the enemy is currently moving

    public float checkPlayerMoveInterval = 0.5f; // Time interval to check if the player has moved

    private Vector3 lastPlayerPosition; // Last known position of the player

    private PlayerMovement playerMovement; // Reference to the player movement script

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gridGenerator = FindObjectOfType<GridGenerator>();
        obstacleManager = FindObjectOfType<ObstacleManager>();
        pathfinder = GetComponent<AStarPathfinding>();

        playerMovement = FindObjectOfType<PlayerMovement>();

        // Initialize last known player position
        lastPlayerPosition = playerTransform.position;

        // Start coroutine to check player movement
        StartCoroutine(CheckPlayerMovement());
       
    }

    void Update()
    {

        // Check if not currently moving and there is a path to follow
        if (!isMoving && currentPath.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator CheckPlayerMovement()
    {
        // Continuously check if the player has moved
        while (true)
        {
            if (playerTransform.position != lastPlayerPosition)
            {
                // Find a path to an adjacent tile to the player's current position
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

        // Follow each waypoint in the path until reaching the destination
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 target = currentPath[currentPathIndex];


            // Move towards the current waypoint
            while (Vector3.Distance(transform.position, target) > stoppingDistance)
            {
                target.y = gridGenerator.cubePrefab.transform.localScale.y; // Keep y position consistent
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++; // Move to the next waypoint in the paths
        }

        isMoving = false; // Reset bool flag when movement is complete
    }

    private Vector3 GetAdjacentTileToPlayer(Vector3 playerPosition)
    {

        // Calculate adjacent tile positions around the player
        Vector2Int playerCoord = WorldToGridCoordinates(playerPosition);
        List<Vector2Int> adjacentCoords = new List<Vector2Int>
        {
            new Vector2Int(playerCoord.x + 1, playerCoord.y),
            new Vector2Int(playerCoord.x - 1, playerCoord.y),
            new Vector2Int(playerCoord.x, playerCoord.y + 1),
            new Vector2Int(playerCoord.x, playerCoord.y - 1)
        };

        // Remove invalid adjacent positions (out of grid bounds or blocked by obstacles)
        adjacentCoords.RemoveAll(coord => coord.x < 0 || coord.x >= gridGenerator.gridWidth || coord.y < 0 || coord.y >= gridGenerator.gridHeight || obstacleManager.IsCellBlocked(coord.x, coord.y));


        // Choose a random valid adjacent position or fallback to player's position
        if (adjacentCoords.Count > 0)
        {
            Vector2Int chosenCoord = adjacentCoords[Random.Range(0, adjacentCoords.Count)];
            return GridToWorldCoordinates(chosenCoord);
        }

        return playerPosition; // Fallback to player's position if no adjacent tiles are valid
    }

    private Vector2Int WorldToGridCoordinates(Vector3 worldPos)
    {

        // Convert world position to grid coordinates
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.spacing);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.spacing); // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorldCoordinates(Vector2Int gridCoord)
    {

        // Convert grid coordinates to world position
        float x = gridCoord.x * gridGenerator.spacing;
        float z = gridCoord.y * gridGenerator.spacing; // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector3(x, gridGenerator.cubePrefab.transform.localScale.y / 2f, z);
    }

    public bool IsOccupied(int x, int y)
    {
        // Check if the enemy occupies a specific grid position
        Vector2Int enemyCoord = WorldToGridCoordinates(transform.position);
        return enemyCoord.x == x && enemyCoord.y == y;
    }
}
