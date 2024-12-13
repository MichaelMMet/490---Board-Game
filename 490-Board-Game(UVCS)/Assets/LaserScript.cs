using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    
    private LineRenderer lineRenderer;
    public Vector3 laserStartPoint;
    public Vector3 laserEndPoint;

    public float laserDuration = 2f;
    private bool laserActive = false;
    private LaserScript laserScript;

    public float maxWidth = 1.5f;
    public float widthIncreaseSpeed = 40f; // Speed at which the laser width increases

    void Start()
    {
        // Get the LineRenderer component attached to this GameObject
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // Start with the laser hidden
        
    }

    void Update()
    {
        if (laserActive)
        {
            // Update the laser's start and end positions
            lineRenderer.SetPosition(0, laserStartPoint);
            lineRenderer.SetPosition(1, laserEndPoint);
        }
    }

    // Call this function to activate the laser and set its target
    public void SetLaserTarget(Vector3 targetPosition)
    {
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
        laserEndPoint = targetPosition;
        laserActive = true;
        lineRenderer.enabled = true; // Show the laser
        StartCoroutine(LaserLifetime());
        StartCoroutine(ExpandLaserWidth());
    }

   // make the laser disappear after a set duration
    private IEnumerator LaserLifetime()
    {
        yield return new WaitForSeconds(laserDuration); 
        laserActive = false;
        lineRenderer.enabled = false; // Hide the laser
    }

    // gradually increase the laser's width over time
    private IEnumerator ExpandLaserWidth()
    {
        float elapsedTime = 0f;
        float initialWidth = lineRenderer.startWidth;

        while (elapsedTime < laserDuration)
        {
            // gradually increase the width of the laser
            float newWidth = Mathf.Lerp(initialWidth, maxWidth, elapsedTime / laserDuration);
            lineRenderer.startWidth = newWidth;
            lineRenderer.endWidth = newWidth;

            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        
        lineRenderer.startWidth = maxWidth;
        lineRenderer.endWidth = maxWidth;
    }
}
