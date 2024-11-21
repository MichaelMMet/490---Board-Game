using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;  // Reference to the grid cell prefab
    public GameObject redDiscPrefab; 
    public GameObject yellowDiscPrefab;

    public int columns = 7;
    public int rows = 6;
    public float spacing = 2.0f; // Space between cells in the grid

    public GameObject[,] grid; // Array to hold references to the grid cells

    void Start()
    {
        grid = new GameObject[columns, rows]; 
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Calculate position for each grid cell (2D array of cells)
                Vector3 position = new Vector3(x * spacing, y * spacing, 0);

                // Instantiate the grid cell at the calculated position
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity);
                grid[x, y] = cell; // Store reference to each grid cell

                // Optionally, make the grid a child of GridManager
                cell.transform.SetParent(transform);

                // Name each cell for easier identification in the hierarchy
                cell.name = "Cell (" + x + ", " + y + ")";
            }
        }
    }

    // Method to check if there is a win condition (4 discs in a row)
    public bool CheckWinCondition()
    {
        // Check horizontal, vertical, and diagonal directions
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x, y].transform.childCount > 0)
                {
                    GameObject currentDisc = grid[x, y].transform.GetChild(0).gameObject;
                    string discColor = currentDisc.CompareTag("Red") ? "Red" : "Yellow";

                    // Check horizontally
                    if (CheckDirection(x, y, 1, 0, discColor))
                        return true;

                    // Check vertically
                    if (CheckDirection(x, y, 0, 1, discColor))
                        return true;

                    // Check diagonal (top-left to bottom-right)
                    if (CheckDirection(x, y, 1, 1, discColor))
                        return true;

                    // Check diagonal (top-right to bottom-left)
                    if (CheckDirection(x, y, 1, -1, discColor))
                        return true;
                }
            }
        }

        return false;
    }

    //check for four consecutive discs in a given direction
    private bool CheckDirection(int startX, int startY, int dx, int dy, string discColor)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            int x = startX + i * dx;
            int y = startY + i * dy;

            // Ensure the position is within bounds
            if (x >= 0 && x < columns && y >= 0 && y < rows)
            {
                // Check if the current cell has a disc and if its color matches
                if (grid[x, y].transform.childCount > 0)
                {
                    GameObject currentDisc = grid[x, y].transform.GetChild(0).gameObject;
                    if ((currentDisc.CompareTag("Red") && discColor == "Red") ||
                        (currentDisc.CompareTag("Yellow") && discColor == "Yellow"))
                    {
                        count++;
                    }
                    else
                    {
                        break; // Discs do not match, so stop checking
                    }
                }
                else
                {
                    break; // No disc found, stop checking
                }
            }
            else
            {
                break; // Out of bounds
            }
        }

        return count == 4; // Check if 4 consecutive discs are found
    }


}