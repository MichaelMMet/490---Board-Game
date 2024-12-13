using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void moveToQuickGame() {
        //SceneManager.LoadSceneAsync(2);
    }
    public void moveToProfile() {
        SceneManager.LoadSceneAsync("ProfileScene");
    }
    public void moveToGameOptions() {
        SceneManager.LoadSceneAsync("GameOptionsScene");
    }
    public void moveToSettings() {
        SceneManager.LoadSceneAsync("SettingsScene");
    }
    public void moveToHomeScreen() {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    public void moveToPopularGame() {
        SceneManager.LoadSceneAsync("BattleshipScene");
    }
}
