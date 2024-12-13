using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// 
/// GameManager handles the main game logic
/// it manages the placement and rotation of ships, turn-based gameplay, hit detection,
/// and game state (player's turn, enemy's turn, game over).
/// 

public class GameManager : MonoBehaviour
{
    //private LaserScript laserScript;
    
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;    

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerShipText;
    public Text enemyShipText;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;

    private bool setupComplete = false;
    private bool playerTurn = true;
    
    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();
    
    private int enemyShipCount = 5;
    private int playerShipCount = 5;

    [Header("Options Panel")]
    public GameObject rematchButton; // Reference to the Rematch Button
    public GameObject quitButton; // Reference to the Quit Button

    public GameObject panel; // Reference to the in game panel
    public bool isShowingPanel = false;


    void Start()
    {
        // initialize the first ship's script and setup button listeners
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayClicked());

        // place enemy ships using the EnemyScript
        enemyShips = enemyScript.PlaceEnemyShips();

        //added
        //laserScript = GameObject.Find("LaserGameObject").GetComponent<LaserScript>();
    }

    // Called when the "Next" button is clicked to place the next ship
    private void NextShipClicked()
    {
        Debug.Log($"NextShipClicked called. touchTiles.Count: {shipScript.TouchTilesCount()}, shipSize: {shipScript.shipSize}");
        if (!shipScript.OnGameBoard())
        {
             Debug.Log("Ship not placed correctly.");
            shipScript.FlashColor(Color.red); // highlight in red if the ship is not placed correctly
        } else
        {
            Debug.Log("Ship placed correctly.");
            if(shipIndex <= ships.Length - 2)
            {
                // Move to the next ship for placement
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                // all ships placed--> transition to gameplay phase
                Debug.Log("All ships placed. Proceeding to gameplay.");
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                topText.text = "guess enemy tile";
                setupComplete = true;

                // hide player's ships for gameplay
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
            }
        }
        
    }

    // Called when a tile is clicked by the player
    public void TileClicked(GameObject tile)
    {
        Debug.Log($"TileClicked called. setupComplete: {setupComplete}, playerTurn: {playerTurn}");
        if(setupComplete && playerTurn)
        {
            // player's turn: launch missile at the clicked tile
            Vector3 tilePos = tile.transform.position;
            tilePos.y += 15;
            playerTurn = false;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
            /*added
            Vector3 targetPosition = gridManager.grid[column, row].transform.position;
            laserScript.SetLaserTarget(targetPosition);
            */
        } else if (!setupComplete)
        {
             // ship placement phase: Place or adjust the current ship           
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    // adjusts the current ship's position based on the clicked tile
    private void PlaceShip(GameObject tile)
    {
        Debug.Log($"Placing ship at tile: {tile.name}");
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    // called when the "Rotate" button is clicked
    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    // checks if a player's missile hit an enemy ship
    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach(int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum))
            {
                // update the tile array and check for ship destruction
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    topText.text = "SUNK!!!";
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "HIT!!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }
            
        }
        if(hitCount == 0)
        {
            // missed the enemy ship
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Missed, there is no ship there";
        }
        Invoke("EndPlayerTurn", 1.5f);
    }

    // handles logic for ending the player's turn
    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }
       Invoke("EndEnemyTurn", 2.0f);
    }

    // handles logic for ending the enemy's turn
    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerShipCount < 1) GameOver("ENEMY WINS!!!");
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        topText.text = "Select a tile";
        playerTurn = true;
        ColorAllTiles(1);
        if (enemyShipCount < 1) GameOver("YOU WIN!!!");
    }

    // updates all tiles colors based on the index
    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    //hHandles the end of the game
    void GameOver(string winner)
    {
        topText.text = "Game Over: " + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    // reloads the current scene to restart the game
    public void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OptionsClicked() {
        isShowingPanel = !(isShowingPanel);
        panel.SetActive(isShowingPanel);
    }

    public void QuitGame() {
        SceneManager.LoadSceneAsync("Main Menu");
    }

}