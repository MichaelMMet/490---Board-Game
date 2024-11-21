using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void moveToQuickGame() {
        SceneManager.LoadSceneAsync(2);
    }
    public void moveToProfile() {
        SceneManager.LoadSceneAsync(3);
    }
    public void moveToGameOptions() {
        SceneManager.LoadSceneAsync(1);
    }
    public void moveToSettings() {
        SceneManager.LoadSceneAsync(4);
    }
    public void moveToHomeScreen() {
        SceneManager.LoadSceneAsync(0);
    }

}
