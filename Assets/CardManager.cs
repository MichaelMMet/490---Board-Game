using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;        // Card prefab with separate front and back quads
    public Sprite[] cardSprites;         // Array of sprites to be selected in the Inspector
    public Texture backTexture;          // Texture to be used for the back of all cards

    private List<CardInstance> instantiatedCards = new List<CardInstance>();

    private float xTrack = -0.5f;

    private void Start()
    {
        // Create cards based on the selected sprites
        for (int i = 0; i < cardSprites.Length; i++)
        {
            Card cardData = GetCardData(cardSprites[i]); // Create Card data using the sprite
            CreateCard(cardData);
        }
    }

    // Method to instantiate a new card and apply front and back textures
    public void CreateCard(Card cardData)
    {
        if (cardPrefab == null || cardData == null)
        {
            Debug.LogWarning("Card prefab or CardData is missing!");
            return;
        }

        // Spawn the card slightly above the table
        Vector3 spawnPosition = new Vector3(xTrack, 1.5f, 0);
        GameObject instantiatedCard = Instantiate(cardPrefab, spawnPosition, Quaternion.identity);
        instantiatedCards.Add(new CardInstance(instantiatedCard, cardData));

        xTrack += 0.5f; // Increment xTrack to position the next card

        // Find front and back renderers after instantiation
        Renderer frontRenderer = instantiatedCard.transform.Find("CardFront").GetComponent<Renderer>();
        Renderer backRenderer = instantiatedCard.transform.Find("CardBack").GetComponent<Renderer>();

        if (frontRenderer != null && backRenderer != null)
        {
            // Apply textures to front and back
            frontRenderer.material.mainTexture = cardData.frontTexture;
            backRenderer.material.mainTexture = backTexture;
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
                backRenderer.material.mainTexture = backTexture;
            }
        }
    }

    // Method to create Card data using a sprite
    private Card GetCardData(Sprite sprite)
    {
        Card card = ScriptableObject.CreateInstance<Card>();
        card.frontTexture = sprite.texture; // Assuming Card has a frontTexture property
        return card;
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
