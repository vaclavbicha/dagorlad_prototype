using UnityEngine;

public class DraggableMovement : MonoBehaviour
{
    public enum Method { Time, Speed, NoMovement }
    public Method CurrentMethod;

    public float TimeSpan;
    public float Speed;
    public Vector2 Destination;

    public float StartTime;
    public Vector2 StartPosition;

    public bool IsLocked;

    private void Awake() {
        StartTime = Time.time;
    }

    private void FixedUpdate() {
        if (CurrentMethod == Method.NoMovement) {
            return;
        }

        switch (CurrentMethod) {
            case Method.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                transform.position = Vector2.Lerp(StartPosition, Destination, tt);
                break;

            case Method.Speed:
                transform.position = Vector2.MoveTowards(StartPosition, Destination, Time.fixedDeltaTime * Speed);
                break;
        }
    }

    public void SetDestination(Vector2 Dest) {
        if (IsLocked) return;

        Destination = Dest;
        StartTime = Time.time;
        StartPosition = transform.position;
    }
}


