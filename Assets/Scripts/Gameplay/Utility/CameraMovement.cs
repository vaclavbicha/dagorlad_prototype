using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraMovement : MonoBehaviour {
    public enum CameraMode { Time, Speed }
    public CameraMode cameraMode;
    public float TimeSpan;
    public float Speed;
    public Vector2 Destination = Vector2.zero;
    public bool Lock;

    [NonSerialized]
    Rigidbody2D rb;
    public float StartTime;
    public Vector2 StartPosition;

    //public bool hasDestinations;
    //public List<GameObject> Destinations = new List<GameObject>();
    //public Queue<Vector2> DestinationsQueue = new Queue<Vector2>();
    //public float CalculatedDistance;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        //if (hasDestinations) {
        //    for (int i = 0; i < Destinations.Count; i++) {
        //        DestinationsQueue.Enqueue(Destinations[i].transform.position);
        //        if (i > 0) CalculatedDistance += Vector2.Distance(Destinations[i - 1].transform.position, Destinations[i].transform.position);
        //    }
        //    Destination = DestinationsQueue.Dequeue();
        //}

        StartTime = Time.time;
        StartPosition = rb.position;
    }

    private void FixedUpdate() {
        Debug.Log(Destination);
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
        if (Lock) return;

        Destination = Dest;
        StartTime = Time.time;
        StartPosition = rb.position;
    }

    //public void SetDestinations(Vector2[] Dests) {
    //    if (Lock) return;

    //    for (int i = 0; i < Dests.Length; i++) {
    //        DestinationsQueue.Enqueue(Dests[i]);
    //    }
    //    Destination = DestinationsQueue.Dequeue();
    //    //Debug.Log(Destination);
    //    hasDestinations = true;
    //    StartTime = Time.time;
    //    StartPosition = rb.position;
    //}

}
