using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 
/// MissileScript handles the behavior of a missile in the game.
/// it checks for collisions with other objects and notifies the GameManager
/// about hits, then destroys itself upon collision.
/// 
public class MissileScript : MonoBehaviour
{
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameManager.CheckHit(collision.gameObject);
        Destroy(gameObject);
    }
}