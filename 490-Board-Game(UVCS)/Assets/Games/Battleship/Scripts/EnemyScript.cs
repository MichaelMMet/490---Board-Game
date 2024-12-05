using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// 
/// EnemyScript handles the logic for the enemy AI 
/// it manages enemy ship placement, turn-based actions (guessing tiles, launching missiles),
/// and updates the enemy's internal grid based on hits and misses.
/// 
public class EnemyScript : MonoBehaviour
{
    // grid representing the enemy's guesses ('o' = open, 'm' = miss, 'h' = hit, 'x' = sunk)
    char[] guessGrid;

    List<int> potentialHits;
    List<int> currentHits;

    private int guess;

    public GameObject enemyMissilePrefab;
    public GameManager gameManager;

    private void Start()
    {
        potentialHits = new List<int>();
        currentHits = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray(); // 100 tiles, all open initially
    }

    /// 
    /// Places enemy ships randomly on the board.
    /// ensures no overlap and that ships stay within the grid boundaries.
    /// 
    public List<int[]> PlaceEnemyShips()
    {
        List<int[]> enemyShips = new List<int[]>
        {
            new int[]{-1, -1, -1, -1, -1}, 
            new int[]{-1, -1, -1, -1},     
            new int[]{-1, -1, -1},        
            new int[]{-1, -1, -1},         
            new int[]{-1, -1}              
        };

        int[] gridNumbers = Enumerable.Range(1, 100).ToArray(); // Available grid positions
        bool taken;

        foreach (int[] tileNumArray in enemyShips)
        {
            taken = true;
            while (taken)
            {
                taken = false;
                int shipNose = UnityEngine.Random.Range(0, 99); // random starting position
                int rotateBool = UnityEngine.Random.Range(0, 2); // randomly horizontal (0) or vertical (1)
                int minusAmount = rotateBool == 0 ? 10 : 1; // determine the step size

                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    // check for out-of-bounds or overlapping tiles
                    if ((shipNose - (minusAmount * i)) < 0 || gridNumbers[shipNose - i * minusAmount] < 0 ||
                        (minusAmount == 1 && shipNose / 10 != ((shipNose - i * minusAmount) - 1) / 10))
                    {
                        taken = true;
                        break;
                    }
                }

                if (!taken)
                {
                    // assign valid positions to the ship
                    for (int j = 0; j < tileNumArray.Length; j++)
                    {
                        tileNumArray[j] = gridNumbers[shipNose - j * minusAmount];
                        gridNumbers[shipNose - j * minusAmount] = -1; // Mark as taken
                    }
                }
            }
        }

        return enemyShips;
    }


    /// Executes the enemy's turn, making a strategic or random guess
    public void NPCTurn()
    {
        List<int> hitIndex = new List<int>();
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') hitIndex.Add(i); // collect all known hit indices
        }

        if (hitIndex.Count > 1)
        {
            // strategy: Continue in the direction of known hits
            int diff = hitIndex[1] - hitIndex[0];
            int posNeg = Random.Range(0, 2) * 2 - 1; // randomly pick forward or backward direction
            int nextIndex = hitIndex[0] + diff;

            while (guessGrid[nextIndex] != 'o')
            {
                if (guessGrid[nextIndex] == 'm' || nextIndex > 100 || nextIndex < 0) diff *= -1;
                nextIndex += diff;
            }

            guess = nextIndex;
        }
        else if (hitIndex.Count == 1)
        {
            // strategy: Guess around a single hit
            List<int> closeTiles = new List<int> { 1, -1, 10, -10 }; // Adjacent tiles
            int index = Random.Range(0, closeTiles.Count);
            int possibleGuess = hitIndex[0] + closeTiles[index];

            // ensure valid guess
            bool onGrid = possibleGuess > -1 && possibleGuess < 100;
            while ((!onGrid || guessGrid[possibleGuess] != 'o') && closeTiles.Count > 0)
            {
                closeTiles.RemoveAt(index);
                index = Random.Range(0, closeTiles.Count);
                possibleGuess = hitIndex[0] + closeTiles[index];
                onGrid = possibleGuess > -1 && possibleGuess < 100;
            }

            guess = possibleGuess;
        }
        else
        {
            // Random guess if no hits exist
            int nextIndex = Random.Range(0, 100);
            while (guessGrid[nextIndex] != 'o') nextIndex = Random.Range(0, 100);
            guess = GuessAgainCheck(nextIndex);
        }

        // Launch a missile at the guessed tile
        GameObject tile = GameObject.Find("Tile (" + (guess + 1) + ")");
        guessGrid[guess] = 'm'; // Mark as missed
        Vector3 vec = tile.transform.position;
        vec.y += 15;
        GameObject missile = Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation);
        missile.GetComponent<EnemyMissileScript>().SetTarget(guess);
        missile.GetComponent<EnemyMissileScript>().targetTileLocation = tile.transform.position;
    }


    /// Ensures the guess is valid and avoids edges or clustering guesses
    private int GuessAgainCheck(int nextIndex)
    {
        int newGuess = nextIndex;
        bool edgeCase = nextIndex < 10 || nextIndex > 89 || nextIndex % 10 == 0 || nextIndex % 10 == 9;
        bool nearGuess = false;

        if (nextIndex + 1 < 100) nearGuess = guessGrid[nextIndex + 1] != 'o';
        if (!nearGuess && nextIndex - 1 > 0) nearGuess = guessGrid[nextIndex - 1] != 'o';
        if (!nearGuess && nextIndex + 10 < 100) nearGuess = guessGrid[nextIndex + 10] != 'o';
        if (!nearGuess && nextIndex - 10 > 0) nearGuess = guessGrid[nextIndex - 10] != 'o';

        if (edgeCase || nearGuess) newGuess = Random.Range(0, 100);
        while (guessGrid[newGuess] != 'o') newGuess = Random.Range(0, 100);

        return newGuess;
    }

    /// updates the grid when a missile hits a player's ship
    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h'; // mark as hit
        Invoke("EndTurn", 1.0f); // delay for turn transition
    }

    /// mrks all known hits as sunk when a ship is destroyed
    public void SunkPlayer()
    {
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') guessGrid[i] = 'x'; // Mark as sunk
        }
    }

    /// ends the enemy's turn and transitions back to the player's turn
    private void EndTurn()
    {
        gameManager.GetComponent<GameManager>().EndEnemyTurn();
    }
        public void PauseAndEnd(int miss)
    {
        if(currentHits.Count > 0 && currentHits[0] > miss)
        {
            foreach(int potential in potentialHits)
            {
                if(currentHits[0] > miss)
                {
                    if (potential < miss) potentialHits.Remove(potential);
                } else
                {
                    if (potential > miss) potentialHits.Remove(potential);
                }
            }
        }
        Invoke("EndTurn", 1.0f);
    }
}
