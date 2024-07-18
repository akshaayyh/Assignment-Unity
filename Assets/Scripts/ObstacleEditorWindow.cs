using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ObstacleEditorWindow : EditorWindow 
{
    public ObstacleData obstacleData; // Reference to the ObstacleData scriptable object


    // Menu item to open the Obstacle Editor window
    [MenuItem("Window/Obstacle Editor")]
    static void ShowWindow()
    {
        GetWindow<ObstacleEditorWindow>("Obstacle Grid Editor"); // Create and show the window
    }

    // GUI method to draw the window interface
    void OnGUI()
    {
        GUILayout.Label("Obstacle Grid Editor", EditorStyles.boldLabel); // Display window title

        // ObstacleData selection field
        obstacleData = (ObstacleData)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleData), false);


        // Display warning if ObstacleData is not assigned
        if (obstacleData == null)
        {
            EditorGUILayout.HelpBox("Please assign an ObstacleData object.", MessageType.Warning);
            return;
        }

        // Iterate through the grid and display toggle buttons for each cell
        for (int x = 0; x < obstacleData.GridSizeX; x++)
        {
            EditorGUILayout.BeginHorizontal();  // Begin horizontal layout group

            for (int y = 0; y < obstacleData.GridSizeY; y++)
            {
                bool isObstacle = obstacleData.GetObstacle(x, y); // Get obstacle state from ObstacleData
                bool newIsObstacle = GUILayout.Toggle(isObstacle, GUIContent.none, GUILayout.Width(30), GUILayout.Height(30)); // Display toggle button


                // Update ObstacleData if toggle state changes
                if (newIsObstacle != isObstacle)
                {
                    obstacleData.SetObstacle(x, y, newIsObstacle);// Set new obstacle state
                    EditorUtility.SetDirty(obstacleData); // Mark the object as dirty to save changes
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // Save changes to assets if GUI has changed
        if (GUI.changed)
        {
            AssetDatabase.SaveAssets();
        }                                         
    }
    }


