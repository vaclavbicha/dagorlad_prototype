using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveTo : MonoBehaviour
{
    public enum Method { Time, Speed, SpeedWithTarget, SpeedWithTargetAndRange, Attacking, NoMovement, SpeedWithFormation }
    public Method method;
    public float TimeSpan;
    public float Speed;

    public bool Lock = false;
    public Vector2 Destination;
    public bool hasReach = false;

    public bool hasDestinations;
    public List<GameObject> Destinations = new List<GameObject>();
    public Queue<Vector2> DestinationsQueue = new Queue<Vector2>();

    public Transform TransformDestination;
    public float StartTime;
    public Vector2 StartPosition;

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
    Vector2 currentRandomTargetPosition;
    public Vector2 offsetRallyPoint;
    public Vector2 offsetedTransformDestination;

    // Nav Mesh and pathfinding
    NavMeshAgent navMeshAgent;

    Vector2? desti = null;

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

    // Start is called before the first frame update
    void Start()
    {
        // setup mav mesh and pathfinding
        if ((navMeshAgent = GetComponent<NavMeshAgent>()) != null) {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            // manual destination setting
            //navMeshAgent.SetDestination(new Vector2(0, 0));
        }

        //On_FinalDestinationReach += PrintValue;

        //if (method == Method.SpeedWithTargetAndRange)
        //{
        //    StartCoroutine(NewLocation(TimeRandomRange));
        //    timer = Time.time + TimeRandomRange;
        //}
    }

    private void FixedUpdate()
    {
        if (transform.position.z == 10) Debug.LogError("WTF");

        if (method == Method.NoMovement) return;

        if (method == Method.SpeedWithTargetAndRange) {
            if (Vector2.Distance(transform.position, TransformDestination.position) < range * 1.42f) {
                if (timer < Time.time) {
                    var signX = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    var signY = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    currentRandomTargetPosition = new Vector2(
                        UnityEngine.Random.Range(TransformDestination.position.x + rangeMin * signX, TransformDestination.position.x + range * signX),
                        UnityEngine.Random.Range(TransformDestination.position.y + rangeMin * signY, TransformDestination.position.y + range * signY));
                    timer = Time.time + TimeRandomRange;
                }
            } else {
                currentRandomTargetPosition = new Vector2(TransformDestination.position.x, TransformDestination.position.y);
            }
        }

        if (method == Method.SpeedWithFormation) {
            offsetedTransformDestination = (Vector2)TransformDestination.position + offsetRallyPoint;

            if (Vector2.Distance(transform.position, offsetedTransformDestination) < range * 1.42f) {
                if (timer < Time.time) {
                    var signX = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    var signY = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    currentRandomTargetPosition = new Vector2(
                        UnityEngine.Random.Range(offsetedTransformDestination.x + rangeMin * signX, offsetedTransformDestination.x + range * signX),
                        UnityEngine.Random.Range(offsetedTransformDestination.y + rangeMin * signY, offsetedTransformDestination.y + range * signY));
                    timer = Time.time + TimeRandomRange;
                }
            } else {
                currentRandomTargetPosition = new Vector2(offsetedTransformDestination.x, offsetedTransformDestination.y);
            }
        }

        if (navMeshAgent && desti != null) return;

        switch (method)
        {
            case Method.Time:
                var tt = (Time.time - StartTime) / TimeSpan;
                rb.MovePosition(Vector2.Lerp(StartPosition, Destination, tt));
                break;

            case Method.Speed:
                rb.MovePosition(Vector2.MoveTowards(rb.position, Destination, Time.fixedDeltaTime * Speed));
                break;

            case Method.SpeedWithTarget:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                setdest((Vector2)TransformDestination.position);
                break;

            case Method.SpeedWithTargetAndRange:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                setdest((Vector2)currentRandomTargetPosition);
                break;

            case Method.Attacking:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                else if(!Lock) setdest((Vector2)TransformDestination.position);
                break;

            case Method.NoMovement:
                break;

            case Method.SpeedWithFormation:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                else setdest((Vector2)currentRandomTargetPosition);
                break;
        }
        
    }

    public void setdest( Vector2 NewDestination) {
        if (!navMeshAgent) return;
        navMeshAgent.SetDestination(NewDestination);
        desti = NewDestination;
    }

    public void SetDestination(Vector2 Dest)
    {
        if (Lock) return;

        Destination = Dest;
        StartTime = Time.time;
        StartPosition = rb.position;
    }

    public void SetDestinations(Vector2[] Dests)
    {
        if (Lock) return;

        for (int i = 0; i < Dests.Length; i++) {
            DestinationsQueue.Enqueue(Dests[i]);
        }
        Destination = DestinationsQueue.Dequeue();
        //Debug.Log(Destination);
        hasDestinations = true;
        StartTime = Time.time;
        StartPosition = rb.position;
    }

    public void PrintValue(GameObject sender)
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        GetComponentInChildren<TextMeshPro>().text = (Time.time - StartTime).ToString("N2");
    }

    IEnumerator NewLocation(float t)
    {
        currentRandomTargetPosition = new Vector2(UnityEngine.Random.Range(TransformDestination.position.x - range, TransformDestination.position.x + range), UnityEngine.Random.Range(TransformDestination.position.y - range, TransformDestination.position.y + range));
        yield return new WaitForSeconds(t);
        StartCoroutine(NewLocation(t));
    }
}
