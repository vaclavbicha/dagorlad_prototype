using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveTo : MonoBehaviour
{
    public enum Method { Time, Speed, SpeedWithTarget, SpeedWithTargetAndRange, NoMovement }
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

    public float range;
    public float rangeMin;
    public float TimeRandomRange;
    float timer;
    Vector3 currentRandomTargetPosition;


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

        //if (method == Method.SpeedWithTargetAndRange)
        //{
        //    StartCoroutine(NewLocation(TimeRandomRange));
        //    timer = Time.time + TimeRandomRange;
        //}
    }
    private void FixedUpdate()
    {
        if(method == Method.SpeedWithTargetAndRange) {
            if (Vector2.Distance(transform.position, TransformDestination.position) < range * 1.42f)
            {
                if(timer < Time.time)
                {
                    var signX = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    var signY = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    currentRandomTargetPosition = new Vector3(
                        UnityEngine.Random.Range(TransformDestination.position.x + rangeMin * signX, TransformDestination.position.x + range * signX),
                        UnityEngine.Random.Range(TransformDestination.position.y + rangeMin * signY, TransformDestination.position.y + range * signY),
                        0);
                    timer = Time.time + TimeRandomRange;
                }
            }
            else
            {
                currentRandomTargetPosition = new Vector3(TransformDestination.position.x, TransformDestination.position.y, 0);
            }
        }

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

            case Method.SpeedWithTargetAndRange:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??");
                else if(!Lock) rb.MovePosition(Vector3.MoveTowards(rb.position, currentRandomTargetPosition, Time.fixedDeltaTime * Speed));
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
    IEnumerator NewLocation(float t)
    {
        currentRandomTargetPosition = new Vector3(UnityEngine.Random.Range(TransformDestination.position.x - range, TransformDestination.position.x + range), UnityEngine.Random.Range(TransformDestination.position.y - range, TransformDestination.position.y + range), 0);
        yield return new WaitForSeconds(t);
        StartCoroutine(NewLocation(t));
    }
}
