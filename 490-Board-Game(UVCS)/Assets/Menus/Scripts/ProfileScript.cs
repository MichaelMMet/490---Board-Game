using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class ProfileScript : MonoBehaviour
{
    string playerId;
    bool b = false;
    [SerializeField] GameObject nameEditorPanel;
    [SerializeField] GameObject editButton;
    [SerializeField] private TMP_InputField inputField; // Drag your InputField here
    [SerializeField] private TextMeshProUGUI nameTextBox;         // Drag your Text component here
    [SerializeField] private TextMeshProUGUI playerIDTextBox;         // Drag your Text component here
    public void moveBack() {
        SceneManager.LoadSceneAsync(0);
    }
    async void Start()
    {
        nameEditorPanel.SetActive(b);

        // Ensure Unity Services are initialized
        await Unity.Services.Core.UnityServices.InitializeAsync();

        // Check if signed in
        if (AuthenticationService.Instance.IsSignedIn)
        {
            // Retrieve the Player ID
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Player is signed in with Player ID: {playerId}");
        }
        else
        {
            Debug.LogError("Player is not signed in.");
        }

        // Update the text box with the player ID
        playerIDTextBox.text = playerId;

    }
    // Called when the input field text is updated
    public void OnEdit()
    {
        b = !b;
        nameEditorPanel.SetActive(b);
        editButton.SetActive(!b);
        Debug.Log("Edit button clicked!");
    }
    public void OnEnterName() {
        // Update the text box with the input field's current text
        nameTextBox.text = inputField.text;

        b = !b;
        nameEditorPanel.SetActive(b);
        editButton.SetActive(!b);
        Debug.Log("Enter Button Clicked!\nName entered: " + inputField.text);
    }
}
