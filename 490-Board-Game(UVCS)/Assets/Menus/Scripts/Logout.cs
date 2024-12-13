using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;

public class Logout : MonoBehaviour
{
    public void logout()
    {
        // Unity Authentication Sign Out
        AuthenticationService.Instance.SignOut();

        Debug.Log("User logged out from Unity Authentication.");
        
        // Update UI or navigate to login scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerAccountsTest");
    }
}
