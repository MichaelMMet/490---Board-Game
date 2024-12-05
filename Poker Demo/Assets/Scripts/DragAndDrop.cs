using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float longClickDuration = 0.5f;
    public float cardOffset = 0.02f; // Offset based on card height to avoid clipping
    public float rotationSpeed = 60f; // Speed of rotation when using the mouse wheel
    public float zoomSpeed = 2f; // Speed of zooming in/out with the mouse wheel

    private Vector3 offset;
    private Vector3 previousMousePos;
    private Vector3 velocity;
    private Rigidbody rb;
    private bool isFlipped = false;
    private bool rightMouseHeld = false;
    private float clickTime;
    private GameObject topCard;
    private Stack<GameObject> cardStack = new Stack<GameObject>();
    private bool rotateKeyPressed = false; // Track if the R key was pressed

    public static bool IsDragging { get; private set; } // Static variable to indicate dragging state

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.drag = 5f;
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
        rb.isKinematic = true;
        offset = transform.position - GetMousePos();
        previousMousePos = Input.mousePosition;
        clickTime = Time.time;
        IsDragging = true;
    }

    private void OnMouseDrag()
    {
        if (Input.GetMouseButtonDown(1) && Input.GetMouseButton(0))
        {
            rightMouseHeld = true;
        }

        if (Input.GetMouseButtonUp(1) && rightMouseHeld)
        {
            FlipObject();
            rightMouseHeld = false;
        }

        Vector3 targetPosition = GetMousePos() + offset;
        transform.position = targetPosition;

        velocity = (Input.mousePosition - previousMousePos) / Time.deltaTime;
        previousMousePos = Input.mousePosition;

        // Rotate the card by 90 degrees when the R key is pressed
        if (Input.GetKeyDown(KeyCode.R) && !rotateKeyPressed)
        {
            transform.Rotate(Vector3.up, 90, Space.World);
            rotateKeyPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            rotateKeyPressed = false;
        }

        // Adjust the card's distance from the camera based on mouse wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 direction = (transform.position - Camera.main.transform.position).normalized;
            transform.position += direction * scroll * zoomSpeed;
        }
    }

    private void OnMouseUp()
    {
        rb.isKinematic = false;
        rb.velocity = Camera.main.ScreenToWorldPoint(velocity) - Camera.main.ScreenToWorldPoint(Vector3.zero);
        rightMouseHeld = false;
        IsDragging = false;

        if (Time.time - clickTime < longClickDuration)
        {
            if (cardStack.Count > 0)
            {
                topCard = cardStack.Pop();
                topCard.transform.SetParent(null);
                topCard.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 1));
                Rigidbody topCardRb = topCard.GetComponent<Rigidbody>();
                if (topCardRb == null)
                {
                    topCardRb = topCard.AddComponent<Rigidbody>();
                }
                topCardRb.isKinematic = false;
                topCardRb.useGravity = true;
            }
        }
    }

    private void FlipObject()
    {
        if (!isFlipped)
        {
            transform.Rotate(0, 180, 0);
            isFlipped = true;
        }
        else
        {
            transform.Rotate(0, -180, 0);
            isFlipped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Card") && !IsDragging)
        {
            cardStack.Push(other.gameObject);
            other.transform.SetParent(transform);

            Vector3 newPosition = new Vector3(0, cardStack.Count * cardOffset, 0);
            other.transform.localPosition = newPosition;

            other.transform.localRotation = Quaternion.identity;

            Rigidbody otherRb = other.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                otherRb.isKinematic = true;
                otherRb.useGravity = false;
            }
        }
    }
}
