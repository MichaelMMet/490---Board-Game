using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For scene management to reload the game

public class PlayerController : MonoBehaviour
{
    //added
    private LaserScript laserScript;
    public GridManager gridManager; // Reference to GridManager
    public GameObject redDiscPrefab; // Red disc prefab for player 1
    public GameObject yellowDiscPrefab; // Yellow disc prefab for player 2

    private bool isPlayerOneTurn = true; // Track current player turn
    private bool isAITurn = false; // Track if it's the AI's turn
    private bool gameIsOver = false; // Flag to track if the game is over
    private bool isRematchInProgress = false; // flag to check if rematch or quit phase

    public TextMeshProUGUI winMessageText; // TextMeshProUGUI to display win message
    public GameObject rematchButton; // Reference to the Rematch Button
    public GameObject quitButton; // Reference to the Quit Button

    public GameObject panel; // Reference to the in game panel
    public bool isShowingPanel = false;

    void Start()
    {
        if (winMessageText != null)
        {
            winMessageText.gameObject.SetActive(false); // Hide the win message at the start
        }
        if (rematchButton != null)
        {
            rematchButton.SetActive(false); // Hide the rematch button at the start
        }
        if (quitButton != null)
        {
            quitButton.SetActive(false); // Hide the quit button at the start
        }
        if (panel != null)
        {
            panel.SetActive(isShowingPanel);
        }

        //added
        laserScript = GameObject.Find("LaserGameObject").GetComponent<LaserScript>();

    }

    void Update()
    {
        // If the game is over, do not allow any further moves
        if (gameIsOver || isRematchInProgress) return;


        if (isAITurn)
        {
            StartCoroutine(AIMoveWithDelay());
            isAITurn = false; // Switch to the next player's turn after AI plays
        }

        // Only listen for clicks if the player is in the game
        if (isPlayerOneTurn && !isAITurn && Input.GetMouseButtonDown(0)) // Left mouse click
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any collider in the scene
            if (Physics.Raycast(ray, out hit))
            {
                // Ensure that the hit object is part of the grid (grid cells)
                if (hit.collider.CompareTag("GridCell"))
                {
                    // Get the column index based on the x position of the hit point
                    // Snapping the mouse position to the closest column
                    float columnPos = hit.point.x;
                    int column = Mathf.RoundToInt((columnPos - gridManager.transform.position.x) / gridManager.spacing);

                    // Ensure the column is within bounds
                    if (column >= 0 && column < gridManager.columns)
                    {
                        StartCoroutine(DropDiscWithDelay(column));
                        isAITurn = true;

                    }
                }
            }
        }
    }

    void DropDisc(int column)
    {
        // Loop through rows from bottom to top
        for (int row = 0; row < gridManager.rows; row++)
        {
            // Check if the current grid cell is empty (has no child)
            if (gridManager.grid[column, row].transform.childCount == 0)
            {
                GameObject discPrefab;
                if (isPlayerOneTurn)
                {
                    discPrefab = redDiscPrefab;
                }
                else
                {
                    discPrefab = yellowDiscPrefab;
                }
                Debug.Log("Disc Dropped by Player " + (isPlayerOneTurn ? "1 (Red)" : "2 (Yellow)"));

                float zOffset = -1f;
                // Instantiate the disc at the position of the grid cell
                GameObject disc = Instantiate(discPrefab, gridManager.grid[column, row].transform.position + new Vector3(0, 0, zOffset), Quaternion.Euler(0,90,0));
                disc.transform.SetParent(gridManager.grid[column, row].transform); // Set disc as a child of the grid cell

                //added
                Vector3 targetPosition = gridManager.grid[column, row].transform.position;
                targetPosition.z = targetPosition.z - 1;
                laserScript.SetLaserTarget(targetPosition);

                // Check if the current move resulted in a win
                if (gridManager.CheckWinCondition())
                {
                    DisplayWinMessage(isPlayerOneTurn ? "Player 1 (Red) Wins!" : "Player 2 (Yellow) Wins!");
                    gameIsOver = true;
                    return; // Stop further moves if someone wins
                }

                isPlayerOneTurn = !isPlayerOneTurn; // Switch turn between players
                
                break; // Stop the loop after placing the disc
            }
        }
    }

    void AIMove()
    {
        List<int> availableColumns = new List<int>();

        // Check each column for available space and adds to list
        for (int column = 0; column < gridManager.columns; column++)
        {
            for (int row = 0; row < gridManager.rows; row++)
            {
                if (gridManager.grid[column, row].transform.childCount == 0)
                {
                    availableColumns.Add(column);
                    break;
                }
            }
        }

        if (availableColumns.Count > 0)
        {
            // drops disc in random column
            int randomColumn = availableColumns[Random.Range(0, availableColumns.Count)];
            StartCoroutine(DropDiscWithDelay(randomColumn));
        }
    }

    // Coroutine to drop disc with delay
    IEnumerator DropDiscWithDelay(int column)
    {
        Debug.Log("Starting the delay before dropping disc");
        DropDisc(column); // Drop the disc
        yield return new WaitForSeconds(2); // Wait for half a second before allowing the next move
        Debug.Log("Ending the delay after dropping disc");
    }

    IEnumerator AIMoveWithDelay()
    {
        // Wait for the previous move to finish
        yield return new WaitForSeconds(1); // 1 seconds delay
        
        if (gameIsOver)
        {
            yield break; // Stop the coroutine if the game is already over
        }
        AIMove();
    }

    // Method to display the win message on the UI
    void DisplayWinMessage(string message)
    {
        if (winMessageText != null)
        {
            winMessageText.text = message; // Set the text of the UI Text element
            winMessageText.gameObject.SetActive(true); // Show the win message
        }

        // Show rematch and quit buttons after the game ends
        if (rematchButton != null)
        {
            rematchButton.SetActive(true); // Show the rematch button
        }
        if (quitButton != null)
        {
            quitButton.SetActive(true); // Show the quit button
        }
    }

    // Method to restart the game (rematch)
    public void Rematch()
    {
        isRematchInProgress = true; // Set the flag to true to disable input
        // Reset the grid
        foreach (var cell in gridManager.grid)
        {
            // Remove all discs from the grid (destroying all child objects in each cell)
            foreach (Transform child in cell.transform)
            {
                Destroy(child.gameObject); // Destroy any discs in the grid
            }
        }

        // Reset game state
        isPlayerOneTurn = true; // Player 1 starts again
        isAITurn = false;
        gameIsOver = false; // Reset the game over flag

        // Hide win message and buttons after rematch
        if (winMessageText != null)
        {
            winMessageText.gameObject.SetActive(false); // Hide win message
        }
        if (rematchButton != null)
        {
            rematchButton.SetActive(false); // Hide rematch button
        }
        if (quitButton != null)
        {
            quitButton.SetActive(false); // Hide quit button
        }

        StartCoroutine(EnableInputAfterReset());

    }

    // Coroutine to enable input after reset
    IEnumerator EnableInputAfterReset()
    {
        yield return new WaitForSeconds(1); // Wait for a second
        isRematchInProgress = false; // Enable input again
    }

    // Method to quit the game
    public void QuitGame()
    {
        /*
        // In Unity editor, quitting will only work if you build the game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quit the application
#endif*/
        SceneManager.LoadSceneAsync("Main Menu"); // Moves back to Home (MainMenu Scene)
    }

    public void showOptions()
    {
        isShowingPanel = !(isShowingPanel);
        panel.SetActive(isShowingPanel);
    }
}