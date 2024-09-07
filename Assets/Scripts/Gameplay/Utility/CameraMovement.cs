using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraMovement : MonoBehaviour {
    public enum CameraMode { Time, Speed }
    public CameraMode cameraMode;
    float TimeSpan;
    public float Speed;
    Vector2 Destination = Vector2.zero;
    public bool IsLocked;

    [NonSerialized]
    Rigidbody2D rb;
    float StartTime;
    Vector2 StartPosition;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        StartTime = Time.time;
        StartPosition = rb.position;
    }

    private void FixedUpdate() {
        switch (cameraMode) {
            case CameraMode.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                rb.MovePosition(Vector2.Lerp(StartPosition, Destination, tt));
                break;

            case CameraMode.Speed:
                rb.MovePosition(Vector2.MoveTowards(rb.position, Destination, Time.fixedDeltaTime * Speed));
                break;
        }
    }

    public void SetDestination(Vector2 Dest) {
        if (IsLocked) return;

        Destination = Dest;
        StartTime = Time.time;
        StartPosition = rb.position;
    }

}
