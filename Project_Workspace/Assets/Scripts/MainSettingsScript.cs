using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSettingsScript : MonoBehaviour
{
    public void moveBack() {
        SceneManager.LoadSceneAsync(0);
    }
}
