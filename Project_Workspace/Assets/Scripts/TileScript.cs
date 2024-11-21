using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    GameManager gameManager;
    Ray ray;
    RaycastHit hit;
    
    private bool missileHit = false;
    Color32[] hitColor = new Color32[2]; // Colors used when a tile is hit

    // Start is called before the first frame update
    void Start()
    {
        // Find and reference the GameManager script attached to the GameManager GameObject
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Create a ray from the camera to the current mouse position
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Check if the raycast hits any collider
        if(Physics.Raycast(ray, out hit))
        {
            // Access hit collider's GameObject properly and compare with this GameObject's name
            if(Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == this.gameObject.name)
            {
                // Ensure the missile hasn't hit this tile yet
                if(missileHit == false)
                {
                    // Call the TileClicked method in GameManager with the hit GameObject
                    gameManager.TileClicked(hit.collider.gameObject);
                }
            }
        }
    }
}