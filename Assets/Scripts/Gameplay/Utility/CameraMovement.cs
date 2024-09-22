using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public enum CameraMode { Time, Speed }
    
    public CameraMode cameraMode;
    public float TimeSpan;
    public float Speed;

    Vector3 StartPosition;
    float StartTime;
    Vector3 Destination;

    private const int cameraZIndex = -10;

    public bool IsLocked { get; set; }

    Vector3 tempPosition;

    float edgeSize = 150f;
    bool isEdgeScrolling;
    float scrollingSpeed = 1.3f;
    float maxScrollingSpeedDifference = 0.3f;

    int screenWidth = Screen.width;
    int screenHeight = Screen.height;

    CameraController cameraController;

    private void Awake() {
        StartTime = Time.time;
        StartPosition = transform.position;
        Destination = StartPosition;
    }

    private void Start() {
        cameraController = GameManager.Instance.GetComponent<CameraController>();
    }

    private void FixedUpdate() {
        switch (cameraMode) {
            case CameraMode.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                tempPosition = Vector3.Lerp(StartPosition, Destination, tt);
                break;

            case CameraMode.Speed:
                tempPosition = Vector3.Lerp(StartPosition, Destination, Time.fixedDeltaTime * Speed);
                break;
        }

        transform.position = new Vector3(tempPosition.x, tempPosition.y, cameraZIndex);

        if (isEdgeScrolling) {
            HandleEdgeScrolling();
        }
    }

    private void HandleEdgeScrolling() {
        Vector3 mousePosition = Input.mousePosition;

        // right
        if (mousePosition.x > screenWidth - edgeSize) {
            float tempX = Destination.x + Mathf.Clamp(Time.deltaTime * scrollingSpeed / (screenWidth - mousePosition.x) * edgeSize, 0, maxScrollingSpeedDifference);
            Destination = cameraController.ClampCamera(new Vector3(tempX, Destination.y, cameraZIndex));
        }

        // left
        if (mousePosition.x < edgeSize) {
            float tempX = Destination.x - Mathf.Clamp(Time.deltaTime * scrollingSpeed / mousePosition.x * edgeSize, 0, maxScrollingSpeedDifference);
            Destination = cameraController.ClampCamera(new Vector3(tempX, Destination.y, cameraZIndex));
        }

        // up
        if (mousePosition.y > screenHeight - edgeSize) {
            float tempY = Destination.y + Mathf.Clamp(Time.deltaTime * scrollingSpeed / (screenHeight - mousePosition.y) * edgeSize, 0, maxScrollingSpeedDifference);
            Destination = cameraController.ClampCamera(new Vector3(Destination.x, tempY, cameraZIndex));
        }

        // down
        if (mousePosition.y < edgeSize) {
            float tempY = Destination.y - Mathf.Clamp(Time.deltaTime * scrollingSpeed / mousePosition.y * edgeSize, 0, maxScrollingSpeedDifference);
            Destination = cameraController.ClampCamera(new Vector3(Destination.x, tempY, cameraZIndex));
        }
    }

    public void SetDestination(Vector3 newDestination) {
        if (IsLocked) return;

        StartTime = Time.time;
        StartPosition = transform.position;
        Destination = new Vector3(newDestination.x, newDestination.y, cameraZIndex);
    }

    public void EnableEdgeScrolling() {
        isEdgeScrolling = true;
    }

    public void DisableEdgeScrolling() { 
        isEdgeScrolling = false; 
    }
}
