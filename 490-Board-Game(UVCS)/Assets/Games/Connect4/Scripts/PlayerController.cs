using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For scene management to reload the game

public class PlayerController : MonoBehaviour
{
    public GridManager gridManager; // Reference to GridManager
    public GameObject redDiscPrefab; // Red disc prefab for player 1
    public GameObject yellowDiscPrefab; // Yellow disc prefab for player 2
    public float discFallSpeed = 0.1f; // Speed of the disc falling

    private bool isPlayerOneTurn = true; // Track current player turn
    public TextMeshProUGUI winMessageText; // TextMeshProUGUI to display win message
    public GameObject rematchButton; // Reference to the Rematch Button
    public GameObject quitButton; // Reference to the Quit Button
    public GameObject panel; // Reference to the in game panel
    public bool isShowingPanel = false;

    void Start()
    {
        // Initially hide the win message and buttons
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
        if (panel != null) {
            panel.SetActive(isShowingPanel);
        }
    }

    void Update()
    {
        // Only listen for clicks if the player is in the game
        if (Input.GetMouseButtonDown(0)) // Left mouse click
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
                        DropDisc(column);
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

                // Instantiate the disc at the position of the grid cell
                GameObject disc = Instantiate(discPrefab, gridManager.grid[column, row].transform.position, Quaternion.identity);
                disc.transform.SetParent(gridManager.grid[column, row].transform); // Set disc as a child of the grid cell

                // Check if the current move resulted in a win
                if (gridManager.CheckWinCondition())
                {
                    DisplayWinMessage(isPlayerOneTurn ? "Player 1 (Red) Wins!" : "Player 2 (Yellow) Wins!");
                    return; // Stop further moves if someone wins
                }

                isPlayerOneTurn = !isPlayerOneTurn; // Switch turn between players
                break; // Stop the loop after placing the disc
            }
        }
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

        // Optionally, reset the grid visuals here if you want
        // Example: you could also reset other visual elements, but this should handle the discs and UI.
    }

    // Method to quit the game
    public void QuitGame()
    {
        /* In Unity editor, quitting will only work if you build the game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quit the application
#endif
*/
        SceneManager.LoadSceneAsync(0); // Moves back to Home (MainMenu Scene)
    }

    public void showOptions() {
        isShowingPanel = !(isShowingPanel);
        panel.SetActive(isShowingPanel);
    }

    

}