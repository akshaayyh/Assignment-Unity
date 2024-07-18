using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MouseRayCast : MonoBehaviour
{
    public Text positionText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Perform a raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit has the TileInfo component
            TileInfo tileInfo = hit.collider.GetComponent<TileInfo>();
            if (tileInfo != null)
            {
                // Output the tile information to the console
                Debug.Log("Mouse over tile at: " + tileInfo.x + ", " + tileInfo.y);

                // Update the UI Text with the tile position
                positionText.text = "Tile Position: " + tileInfo.x + ", " + tileInfo.y;
            }
        }
    }
}
