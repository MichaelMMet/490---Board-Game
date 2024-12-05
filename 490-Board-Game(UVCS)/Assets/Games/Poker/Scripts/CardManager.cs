using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;        // Card prefab with separate front and back quads
    public string cardFolder = "Poker Cards"; // Folder containing the card ScriptableObjects
    public float respawnThreshold = 10f; // Maximum allowed distance from the table before respawning

    private List<CardInstance> instantiatedCards = new List<CardInstance>();

    private Vector3 deckPosition = new Vector3(0, 1.5f, 0); // Initial position to spawn the deck
    private float yOffset = 0.01f; // Offset to stack cards vertically

    private void Start()
    {
        // Load all card ScriptableObjects from the specified folder
        Card[] cardDataArray = Resources.LoadAll<Card>(cardFolder);
        // Debug statement to log the number of cards loaded
        Debug.Log($"Loaded {cardDataArray.Length} cards from folder: {cardFolder}");

        // Create cards based on the loaded ScriptableObjects
        foreach (Card cardData in cardDataArray)
        {
            CreateCard(cardData);
        }
    }

    private void Update()
    {
        CheckCardPositions();
    }

    // Method to instantiate a new card and apply front and back textures
    public void CreateCard(Card cardData)
    {
        if (cardPrefab == null || cardData == null)
        {
            Debug.LogWarning("Card prefab or CardData is missing!");
            return;
        }

        // Spawn the card at the deck position
        GameObject instantiatedCard = Instantiate(cardPrefab, deckPosition, Quaternion.identity);
        instantiatedCards.Add(new CardInstance(instantiatedCard, cardData));

        // Increment the y-coordinate for the next card to stack them vertically
        deckPosition.y += yOffset;

        // Find front and back renderers after instantiation
        Renderer frontRenderer = instantiatedCard.transform.Find("CardFront").GetComponent<Renderer>();
        Renderer backRenderer = instantiatedCard.transform.Find("CardBack").GetComponent<Renderer>();

        if (frontRenderer != null && backRenderer != null)
        {
            // Apply textures to front and back
            frontRenderer.material.mainTexture = cardData.frontTexture;
            backRenderer.material.mainTexture = cardData.backTexture;
        }
        else
        {
            Debug.LogWarning("Front or back renderer is missing on the card prefab.");
        }
    }

    // Method to apply a texture to the card
    public void ApplyTexture(Texture texture)
    {
        foreach (var cardInstance in instantiatedCards)
        {
            Renderer renderer = cardInstance.CardObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = texture;
            }
        }
    }

    // Swap to front texture
    public void ShowFront()
    {
        foreach (var cardInstance in instantiatedCards)
        {
            Renderer frontRenderer = cardInstance.CardObject.transform.Find("CardFront").GetComponent<Renderer>();
            if (frontRenderer != null)
            {
                frontRenderer.material.mainTexture = cardInstance.CardData.frontTexture;
            }
        }
    }

    // Swap to back texture
    public void ShowBack()
    {
        foreach (var cardInstance in instantiatedCards)
        {
            Renderer backRenderer = cardInstance.CardObject.transform.Find("CardBack").GetComponent<Renderer>();
            if (backRenderer != null)
            {
                backRenderer.material.mainTexture = cardInstance.CardData.backTexture;
            }
        }
    }

    // Method to check the position of each card and respawn if necessary
    private void CheckCardPositions()
    {
        foreach (var cardInstance in instantiatedCards)
        {
            if (Vector3.Distance(cardInstance.CardObject.transform.position, deckPosition) > respawnThreshold)
            {
                RespawnCard(cardInstance);
            }
        }
    }

    // Method to respawn a card on top of the deck
    private void RespawnCard(CardInstance cardInstance)
    {
        cardInstance.CardObject.transform.position = deckPosition;
        cardInstance.CardObject.transform.rotation = Quaternion.identity;
        deckPosition.y += yOffset; // Increment the y-coordinate for the next card
    }

    // Custom class to hold the instantiated card GameObject and its associated Card data
    private class CardInstance
    {
        public GameObject CardObject { get; }
        public Card CardData { get; }

        public CardInstance(GameObject cardObject, Card cardData)
        {
            CardObject = cardObject;
            CardData = cardData;
        }
    }
}
