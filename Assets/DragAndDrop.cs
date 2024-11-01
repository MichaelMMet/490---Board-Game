using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 previousMousePos;
    private Vector3 velocity;
    private Rigidbody rb;
    private bool isFlipped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // Optional: Disable gravity if not needed
            rb.drag = 5f;          // Adjust this for smoother deceleration
        }
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseDown()
    {
        rb.isKinematic = true; // Stop physics while dragging
        offset = transform.position - GetMousePos();
        previousMousePos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        // Check for right-click while holding left-click to flip the object
        if (Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            FlipObject();
        }

        // Update position based on mouse movement
        Vector3 targetPosition = GetMousePos() + offset;
        transform.position = targetPosition;

        // Calculate velocity based on mouse movement
        velocity = (Input.mousePosition - previousMousePos) / Time.deltaTime;
        previousMousePos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        rb.isKinematic = false; // Re-enable physics
        rb.velocity = Camera.main.ScreenToWorldPoint(velocity) - Camera.main.ScreenToWorldPoint(Vector3.zero);
    }

    private void FlipObject()
    {
        if (!isFlipped)
        {
            transform.Rotate(0, 180, 0); // Rotate the object by 180 degrees around Y-axis
            isFlipped = true;
        }
        else
        {
            transform.Rotate(0, -180, 0); // Rotate back if already flipped
            isFlipped = false;
        }
    }
}
