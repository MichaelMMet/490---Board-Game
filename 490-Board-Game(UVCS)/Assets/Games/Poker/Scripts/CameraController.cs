using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // The point the camera rotates around
    public float distance = 5f;  // Distance from the target (arbitrary)
    public float rotationSpeed = 30f;  // Camera rotation speed (arbitrary)
    public float zoomSpeed = 2f;  // Zoom speed (arbitrary)
    public float minDistance = 0.10f;  // Minimum zoom distance
    public float maxDistance = 5f;  // Maximum zoom distance
    public float angleX;  // Fixed X rotation angle (elevation) (arbitrary)
    private float angleY;  // Current Y rotation angle

    void Start()
    {
        angleY = transform.eulerAngles.y; // Use the current Y rotation as starting angle
        angleX = 30f; // Set the X angle to an arbitrary amount
    }

    void Update()
    {
        // Check if a card is being dragged
        if (DragAndDrop.IsDragging)
        {
            return; // Do not update the camera if a card is being dragged
        }

        // Update horizontal angle based on input
        angleY += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        // Update distance based on mouse wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Set new camera position and rotation
        Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
        transform.position = target.position + rotation * new Vector3(0, 0, -distance);
        transform.LookAt(target);
    }
}
