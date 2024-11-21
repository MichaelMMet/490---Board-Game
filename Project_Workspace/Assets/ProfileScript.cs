using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileScript : MonoBehaviour
{
    public void moveBack() {
        SceneManager.LoadSceneAsync(0);
    }
}
