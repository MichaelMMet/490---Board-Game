using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] ships;
    private bool setupComplete =false;
    private bool playerTurn=true;
    private int shipIndex = 0;
    private ShipScript shipScript;

    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
    }

    void Update()
    {
        
    }

    public void TileClicked(GameObject tile)
    {
        if(setupComplete && playerTurn) 
        {
            // drop a missle 
        } else if(!setupComplete)
        {
            PlaceShip(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position); 
        ships[shipIndex].transform.localPosition = newVec;
    }
}

