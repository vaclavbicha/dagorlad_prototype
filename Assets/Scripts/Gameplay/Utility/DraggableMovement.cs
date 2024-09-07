using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DraggableMovement : MonoBehaviour
{
    public enum Method { Time, Speed, NoMovement }
    public Method CurrentMethod;

    public float TimeSpan;
    public float Speed;
    public Vector2 Destination;

    public float StartTime;
    public Vector2 StartPosition;

    [NonSerialized]
    public Rigidbody2D rb;

    public bool IsLocked;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        StartTime = Time.time;
        StartPosition = rb.position;
    }

    private void FixedUpdate() {
        if (CurrentMethod == Method.NoMovement) {
            return;
        }

        switch (CurrentMethod) {
            case Method.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                rb.MovePosition(Vector2.Lerp(StartPosition, Destination, tt));
                break;

            case Method.Speed:
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


