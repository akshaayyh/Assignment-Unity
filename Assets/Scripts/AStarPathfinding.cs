using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    private GridGenerator gridGenerator;     // Reference to the grid generator script
    private ObstacleManager obstacleManager; // Reference to obstacle manager script

    private void Start()
    {
        // Find and assign references to GridGenerator and ObstacleManager scripts
        gridGenerator = FindObjectOfType<GridGenerator>();
        obstacleManager = FindObjectOfType<ObstacleManager>();
    }

    // A* Pathfinding method to find a path from startPos to targetPos
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Convert world positions to grid coordinates
        Vector2Int startCoord = WorldToGridCoordinates(startPos);
        Vector2Int targetCoord = WorldToGridCoordinates(targetPos);

        // Initialize open and closed lists
        List<Vector2Int> openList = new List<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        openList.Add(startCoord);

        // Parent dictionary to store the path
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Cost dictionaries
        Dictionary<Vector2Int, float> gCost = new Dictionary<Vector2Int, float>();
        gCost[startCoord] = 0f;

        Dictionary<Vector2Int, float> fCost = new Dictionary<Vector2Int, float>();
        fCost[startCoord] = Vector2Int.Distance(startCoord, targetCoord); // Heuristic cost (Manhattan distance)

        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost from the open list
            Vector2Int current = GetLowestFCostNode(openList, fCost);

            // If reached the target position, reconstruct path
            if (current == targetCoord)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedSet.Add(current);

            // Get neighbors of current node
            List<Vector2Int> neighbors = GetNeighbors(current);


            // Process each neighbor
            foreach (var neighbor in neighbors)
            {
                // Skip neighbor if it's in the closed set or is blocked by an obstacle
                if (closedSet.Contains(neighbor) || obstacleManager.IsCellBlocked(neighbor.x, neighbor.y)) continue;

                float tentativeGCost = gCost[current] + Vector2Int.Distance(current, neighbor);


                // Update path if this path to neighbor is shorter
                if (!openList.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + Vector2Int.Distance(neighbor, targetCoord);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // If no path found, return an empty list
        return new List<Vector3>();
    }

    // Helper method to convert world position to grid coordinates
    private Vector2Int WorldToGridCoordinates(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.spacing);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.spacing); // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector2Int(x, y);
    }

    // Helper method to get neighbors of a grid coordinate
    private List<Vector2Int> GetNeighbors(Vector2Int coord)
    {
        


        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Check the four cardinal directions
        if (coord.x + 1 < gridGenerator.gridWidth) neighbors.Add(new Vector2Int(coord.x + 1, coord.y));
        if (coord.x - 1 >= 0) neighbors.Add(new Vector2Int(coord.x - 1, coord.y));
        if (coord.y + 1 < gridGenerator.gridHeight) neighbors.Add(new Vector2Int(coord.x, coord.y + 1));
        if (coord.y - 1 >= 0) neighbors.Add(new Vector2Int(coord.x, coord.y - 1));

        return neighbors;

    }

    // Helper method to get the node with the lowest fCost from the open list
    private Vector2Int GetLowestFCostNode(List<Vector2Int> openList, Dictionary<Vector2Int, float> fCost)
    {
        Vector2Int lowestNode = openList[0];
        float lowestCost = fCost[lowestNode];

        foreach (var node in openList)
        {
            if (fCost[node] < lowestCost)
            {
                lowestNode = node;
                lowestCost = fCost[node];
            }
        }

        return lowestNode;
    }

    // Helper method to reconstruct the path from start to end using cameFrom dictionary
    private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(GridToWorldCoordinates(current));

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(GridToWorldCoordinates(current));
        }

        path.Reverse(); // Reverse the path to get it from start to end

        return path;
    }

    // Helper method to convert grid coordinates to world position
    private Vector3 GridToWorldCoordinates(Vector2Int gridCoord)
    {
        float x = gridCoord.x * gridGenerator.spacing;
        float z = gridCoord.y * gridGenerator.spacing; // Assuming z-axis corresponds to y-coordinate in grid
        return new Vector3(x, gridGenerator.cubePrefab.transform.localScale.y / 2f, z);
    }
}
