using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;             // Movement speed of the player
    public float stoppingDistance = 0.1f;    // Distance at which player stops moving
    private bool isMoving;                   // Flag to check if the player is currently moving
    private GridGenerator gridGenerator;     // Reference to the grid generator script
    private ObstacleManager obstacleManager; // Reference to obstacle manager script
    private AStarPathfinding pathfinder;     // Reference to A* pathfinding script
          
    private LineRenderer lineRenderer;       // Reference to the LineRenderer component
    private EnemyMovement enemyMovement;     // Reference to the enemy movement script

    private List<Vector3> currentPath = new List<Vector3>();  // Current path to follow
    private int currentPathIndex;             // Index of the current waypoint in the path
    void Start()
    {

        // Initialize references to other components
        gridGenerator = FindObjectOfType<GridGenerator>();
        obstacleManager = FindObjectOfType<ObstacleManager>();
        pathfinder = GetComponent<AStarPathfinding>();
        lineRenderer = GetComponent<LineRenderer>();
        enemyMovement = FindObjectOfType<EnemyMovement>();

     
    }

    void Update()
    {
        // Check for input only if not currently moving
        if (!isMoving && Input.GetMouseButton(0)) 
        {
            // Perform raycast from mouse position to select a tile
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object hit has the TileInfo component
                TileInfo tileInfo = hit.collider.GetComponent<TileInfo>();
                if (tileInfo != null)
                {
                    int targetX = tileInfo.x;
                    int targetY = tileInfo.y;

                    // Check if the target tile is walkable (not an obstacle) and not occupied by the enemy
                    if (!obstacleManager.IsCellBlocked(targetX, targetY) && !enemyMovement.IsOccupied(targetX, targetY)) 
                    {
                        // Calculate target position based on grid spacing
                        Vector3 targetWorldPosition = gridGenerator.GetWorldPosition(targetX, targetY);
                        Vector3 cubeTopPosition = targetWorldPosition + Vector3.up * (gridGenerator.cubePrefab.transform.localScale.y / 2f); // Adjust for player's height

                        // Find path to the target position
                        currentPath = pathfinder.FindPath(transform.position, cubeTopPosition);

                       

                        if (currentPath.Count > 0)
                        {
                            currentPathIndex = 0;
                           
                            StartCoroutine(FollowPath());
                           
                        }
                        
                    }
                    else
                    {
                        Debug.Log("Cannot move to an obstacle!");
                    }
                }
            }
            
        }
        
    }

    IEnumerator FollowPath()
    {
        isMoving = true;

        // Follow each waypoint in the path until reaching the destination
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 target = currentPath[currentPathIndex];

            // Move towards the current waypoint
            while (Vector3.Distance(transform.position, target) > stoppingDistance)
            {
                target.y = gridGenerator.cubePrefab.transform.localScale.y;
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

                // Update rotation to face the next waypoint if available
                if (currentPathIndex + 1 < currentPath.Count)
                {
                    FaceTowardsNextWaypoint();
                }

                yield return null; 
            }
            // Move to the next waypoint in the path
            currentPathIndex++;

            // Update the LineRenderer to visualize the path
            UpdateLineRenderer();
        }
        // set movement flags to false as mmovement complete
        isMoving = false;

        // Clear the LineRenderer
        lineRenderer.positionCount = 0;

       
    }
    void UpdateLineRenderer()
    {
        // Update the LineRenderer to visualize the remaining path
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = currentPath.Count - currentPathIndex;
            for (int i = currentPathIndex; i < currentPath.Count; i++)
            {

                lineRenderer.SetPosition(i - currentPathIndex, currentPath[i]);
            }
        }
    }
    void FaceTowardsNextWaypoint()
    {
        // Rotate towards the next waypoint in the path 

        Vector3 nextWaypointDirection = (currentPath[currentPathIndex + 1] - transform.position).normalized;
        nextWaypointDirection.y = 0f; // Restrict rotation to horizontal plane

        Quaternion targetRotation = Quaternion.LookRotation(nextWaypointDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust rotation speed as needed
    }


}
