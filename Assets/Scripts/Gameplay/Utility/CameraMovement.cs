using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public enum CameraMode { Time, Speed }
    public CameraMode cameraMode;
    public float TimeSpan;
    public float Speed;
    Vector2 Destination;
    public bool IsLocked;

    float StartTime;
    Vector2 StartPosition;

    Vector2 tempPosition;

    private void Awake() {
        StartTime = Time.time;
        StartPosition = transform.position;
    }

    private void FixedUpdate() {
        switch (cameraMode) {
            case CameraMode.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                tempPosition = Vector2.Lerp(StartPosition, Destination, tt);
                break;

            case CameraMode.Speed:
                tempPosition = Vector2.MoveTowards(StartPosition, Destination, Time.fixedDeltaTime * Speed);
                break;
        }

        transform.position = new Vector3(tempPosition.x, tempPosition.y, -10);
    }

    public void SetDestination(Vector2 newDestination) {
        if (IsLocked) return;

        Destination = newDestination;
        StartTime = Time.time;
        StartPosition = transform.position;
    }
}
