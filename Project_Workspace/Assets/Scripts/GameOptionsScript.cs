using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOptionsScript : MonoBehaviour
{
    public void moveBack() {
        SceneManager.LoadSceneAsync(0);
    }
    public void moveToBattleship() {
        SceneManager.LoadSceneAsync(6);
    }
    public void moveToConnect4() {
        SceneManager.LoadSceneAsync(5);
    }
    public void moveToPoker() {
        SceneManager.LoadSceneAsync(2);
    }
}
