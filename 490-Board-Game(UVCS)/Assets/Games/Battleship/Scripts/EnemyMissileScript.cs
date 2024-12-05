using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// EnemyMissileScript handles the behavior of enemy missiles in the game.
/// it tracks the missile's target, detects collisions with ships or tiles, 
/// and communicates with the GameManager and EnemyScript to manage game logic
/// 
public class EnemyMissileScript : MonoBehaviour
{
    GameManager gameManager;
    EnemyScript enemyScript;
    public Vector3 targetTileLocation;
    private int targetTile = -1;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyScript = GameObject.Find("Enemy").GetComponent<EnemyScript>();

    }

    /// handles collision events when the missile hits another object
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            if (collision.gameObject.name == "Submarine") targetTileLocation.y += 0.3f;

            // notify the GameManager that the enemy hit a player's ship
            gameManager.EnemyHitPlayer(targetTileLocation, targetTile, collision.gameObject);
        }
        else
        {
            // if the missile misses, notify the EnemyScript to handle the turn end
            enemyScript.PauseAndEnd(targetTile);
        }

        // destroy the missile after handling the collision
        Destroy(gameObject);
    }

    /// sets the target tile index for the missile.
    public void SetTarget(int target)
    {
        targetTile = target;
    }
}