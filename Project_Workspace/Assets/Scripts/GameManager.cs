using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For scene management to reload the game

public class GameManager : MonoBehaviour
{
    public GameObject[] ships;
    private bool setupComplete =false;
    private bool playerTurn=true;
    private int shipIndex = 0;
    private ShipScript shipScript;
    public GameObject panel; // Reference to the in game panel
    public bool isShowingPanel = false;


    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();

        if (panel != null) {
            panel.SetActive(isShowingPanel);
        }
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

    public void showOptions() {
        isShowingPanel = !(isShowingPanel);
        panel.SetActive(isShowingPanel);
    }

    public void quitGame() {
        SceneManager.LoadSceneAsync(0);
    }
    public void restartGame() {
        SceneManager.LoadSceneAsync(6);
    }

}

