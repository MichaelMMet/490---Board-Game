using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOptionsScript : MonoBehaviour
{
    public void moveBack() {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    public void moveToBattleship() {
        SceneManager.LoadSceneAsync("BattleshipScene");
    }
    public void moveToConnect4() {
        SceneManager.LoadSceneAsync("Connect4Scene");
    }
    public void moveToPoker() {
        SceneManager.LoadSceneAsync("Poker");
    }
}
