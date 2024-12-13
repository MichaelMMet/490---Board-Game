using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisibility : MonoBehaviour
{
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        Debug.Log($"{gameObject.name}: Renderer components initialized.");
    }

    public void SetVisibility(bool isVisible)
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
            Debug.Log($"{gameObject.name}: Renderer components re-initialized.");
        }

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
            Debug.Log($"{gameObject.name}: Renderer {renderer.name} visibility set to {isVisible}.");
        }
    }
}
