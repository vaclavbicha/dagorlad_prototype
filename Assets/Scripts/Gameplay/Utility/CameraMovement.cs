using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public enum CameraMode { Time, Speed }

    private const int cameraZIndex = -10;

    public CameraMode cameraMode;
    public float TimeSpan;
    public float Speed;

    Vector3 StartPosition;
    float StartTime;
    Vector3 Destination;

    public bool IsLocked;

    Vector3 tempPosition;

    private void Awake() {
        StartTime = Time.time;
        StartPosition = transform.position;
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
    }

    public void SetDestination(Vector3 newDestination) {
        if (IsLocked) return;

        StartTime = Time.time;
        StartPosition = transform.position;
        Destination = new Vector3(newDestination.x, newDestination.y, cameraZIndex);
    }
}
