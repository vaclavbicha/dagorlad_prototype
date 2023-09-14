using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveTo : MonoBehaviour
{
    public enum Method { Time, Speed, SpeedWithTarget, NoMovement }
    public Method method;
    public float TimeSpan;
    public float Speed;

    public bool Lock = false;
    public Vector3 Destination;
    public bool hasReach = false;

    public bool hasDestinations;
    public List<GameObject> Destinations = new List<GameObject>();
    public Queue<Vector3> DestinationsQueue = new Queue<Vector3>();

    public Transform TransformDestination;
    public float StartTime;
    public Vector3 StartPosition;

    public delegate void TargetEventDelegate(GameObject sender);
    public event TargetEventDelegate On_FinalDestinationReach;
    public event TargetEventDelegate On_DestinationReach;

    [NonSerialized]
    public Rigidbody2D rb;

    public float Distance;
    public float CalculatedDistance;
    public float TravelTime;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (hasDestinations)
        {
            for (int i = 0; i < Destinations.Count; i++)
            {
                DestinationsQueue.Enqueue(Destinations[i].transform.position);
                if (i > 0) CalculatedDistance += Vector2.Distance(Destinations[i - 1].transform.position, Destinations[i].transform.position);
            }
            Destination = DestinationsQueue.Dequeue();
        }
        StartTime = Time.time;
        StartPosition = rb.position;
    }
    void Start()
    {
        //On_FinalDestinationReach += PrintValue;
    }
    private void FixedUpdate()
    {
        switch (method)
        {
            case Method.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                rb.MovePosition(Vector3.Lerp(StartPosition, Destination, tt));
                break;

            case Method.Speed:
                rb.MovePosition(Vector3.MoveTowards(rb.position, Destination, Time.fixedDeltaTime * Speed));
                break;

            case Method.SpeedWithTarget:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??");
                else rb.MovePosition(Vector3.MoveTowards(rb.position, TransformDestination.position, Time.fixedDeltaTime * Speed));
                break;

            case Method.NoMovement:
                break;

        }
        if (rb.position == (Vector2)Destination)
        {
            if (!hasReach)
            {
                if (hasDestinations)
                {
                    if (DestinationsQueue.Count != 0)
                    {
                        // To measure distance b2een start and finish
                        var destCopy = Destination;
                        //

                        Destination = DestinationsQueue.Dequeue();
                        On_DestinationReach?.Invoke(gameObject);

                        // To measure distance b2een start and finish
                        Distance += Vector2.Distance(destCopy, Destination);
                        if (TravelTime == 0) TravelTime = Time.time;
                        //
                    }
                    else
                    {
                        //Debug.Log("Final destination reached");
                        hasReach = true;
                        On_FinalDestinationReach?.Invoke(gameObject);

                        // To measure distance b2een start and finish
                        TravelTime = Time.time - TravelTime;
                        //
                    }
                }
                else
                {
                    hasReach = true;
                    On_FinalDestinationReach?.Invoke(gameObject);
                }
                StartTime = Time.time;
                StartPosition = rb.position;
            }
        }
        else
        {
            hasReach = false;
        }
    }
    public void SetDestination(Vector3 Dest)
    {
        if (!Lock)
        {
            Destination = Dest;
            StartTime = Time.time;
            StartPosition = rb.position;
        }
    }
    public void SetDestinations(Vector3[] Dests)
    {
        if (!Lock)
        {
            for (int i = 0; i < Dests.Length; i++)
            {
                DestinationsQueue.Enqueue(Dests[i]);
            }
            Destination = DestinationsQueue.Dequeue();
            //Debug.Log(Destination);
            hasDestinations = true;
            StartTime = Time.time;
            StartPosition = rb.position;
        }
    }
    public void PrintValue(GameObject sender)
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        GetComponentInChildren<TextMeshPro>().text = (Time.time - StartTime).ToString("N2");
    }
}
