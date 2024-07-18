using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



// ScriptableObject to hold obstacle grid data
[CreateAssetMenu(fileName ="New ObstacleData", menuName = "ScriptableObjects/ObstacleData")]
public class ObstacleData : ScriptableObject
{

    // array data structre to hold obstacle grid
    private bool[,] obstacleGrid=new bool[10,10];


    // Event triggered when obstacle data changes
    public UnityEvent OnObstacleDataChanged = new UnityEvent();

    // Method to get obstacle state at grid position (x, y)
    public bool GetObstacle(int x, int y)
    {
        return obstacleGrid[x, y];
    }

    // Method to set obstacle state at grid position (x, y)
    public void SetObstacle(int x, int y, bool isObstacle)
    {
        obstacleGrid[x, y] = isObstacle;
        OnObstacleDataChanged.Invoke();
    }

    // Property to get the size of the grid in X direction and y direction
    public int GridSizeX => obstacleGrid.GetLength(0);
    public int GridSizeY => obstacleGrid.GetLength(1);
}
