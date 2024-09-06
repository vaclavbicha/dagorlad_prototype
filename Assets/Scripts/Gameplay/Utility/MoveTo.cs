using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveTo : MonoBehaviour
{
    public enum Method { Time, Speed, SpeedWithTarget, SpeedWithTargetAndRange, Attacking, NoMovement, SpeedWithFormation }
    public Method CurrentMethod;
    private Method previousMethod; // for changes in the inspector

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

    bool isPathDestinationSet;
    private float checkRadius = 5f;
    private float separationRadius = 1.5f;

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
        if (CurrentMethod == Method.NoMovement) {
            ClearPathDestination();
            return;
        }

        if (isPathDestinationSet) {
            SearchForNearbyUnits();
            return;
        }

        switch (CurrentMethod)
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
                SetPathDestination((Vector2)TransformDestination.position);
                break;

            case Method.SpeedWithTargetAndRange:
                CalculateRandomPosition_SpeedWithTargetAndRange();
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                SetPathDestination(currentRandomTargetPosition);
                break;

            case Method.Attacking:
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                else if(!Lock) SetPathDestination((Vector2)TransformDestination.position);
                break;

            case Method.SpeedWithFormation:
                CalculateRandomPosition_SpeedWithFormation();
                if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);
                else SetPathDestination(currentRandomTargetPosition);
                break;
        }
        
    }

    private void CalculateRandomPosition_SpeedWithTargetAndRange() {
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

    private void CalculateRandomPosition_SpeedWithFormation() {
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

    public void SetPathDestination(Vector2 NewDestination) {
        if (!navMeshAgent) return;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(NewDestination);
        isPathDestinationSet = true;
    }

    private void ClearPathDestination() {
        if (!navMeshAgent) return;
        navMeshAgent.isStopped = true;
        isPathDestinationSet = false;
    }

    private void SearchForNearbyUnits() {
        if (!navMeshAgent) return;

        if (Vector2.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance) {
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
            
            foreach (Collider2D unit in nearbyUnits) {
                float ratio = Mathf.Clamp01((unit.transform.position - transform.position).magnitude / separationRadius) * 10;
                Destination -= ratio * (Vector2)(unit.transform.position - transform.position).normalized;
            }
        }
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

    // Updating method when changed in the inspector
    private void OnValidate() {
        if (previousMethod != CurrentMethod) {
            previousMethod = CurrentMethod;
            ClearPathDestination();
        }
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
