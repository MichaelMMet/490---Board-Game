using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occluder : MonoBehaviour
{
    private List<CardVisibility> cardsInOccluder = new List<CardVisibility>();
    private bool isCameraInside = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called with {other.name}");
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("MainCamera entered the occluder.");
            isCameraInside = true;
            SetCardsVisibility(true);
        }
        else if (other.CompareTag("Card"))
        {
            CardVisibility cardVisibility = other.GetComponent<CardVisibility>();
            if (cardVisibility != null)
            {
                cardsInOccluder.Add(cardVisibility);
                Debug.Log($"Card {other.name} entered the occluder.");
                if (!isCameraInside)
                {
                    cardVisibility.SetVisibility(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit called with {other.name}");
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("MainCamera exited the occluder.");
            isCameraInside = false;
            SetCardsVisibility(false);
        }
        else if (other.CompareTag("Card"))
        {
            CardVisibility cardVisibility = other.GetComponent<CardVisibility>();
            if (cardVisibility != null)
            {
                cardsInOccluder.Remove(cardVisibility);
                Debug.Log($"Card {other.name} exited the occluder.");
                cardVisibility.SetVisibility(true);
            }
        }
    }

    private void SetCardsVisibility(bool isVisible)
    {
        Debug.Log($"Setting visibility of {cardsInOccluder.Count} cards to {isVisible}.");
        foreach (CardVisibility card in cardsInOccluder)
        {
            card.SetVisibility(isVisible);
        }
    }
}
