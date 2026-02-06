using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement : MonoBehaviour
{
    public enum Method { Time, Speed, SpeedWithTarget, SpeedWithTargetAndRange, Attacking, NoMovement, SpeedWithFormation }
    public Method CurrentMethod;
    private Method previousMethod; // for changes in the inspector

    public float TimeSpan;
    public float Speed;

    public bool IsLocked = false;
    public Vector2 Destination;

    public bool hasDestinations;
    public List<GameObject> Destinations = new();
    public Queue<Vector2> DestinationsQueue = new();

    public Transform TransformDestination;
    public float StartTime;
    public Vector2 StartPosition;

    public float range;
    public float rangeMin;
    public float TimeRandomRange;
    float timer;
    Vector2 currentRandomTargetPosition;
    public Vector2 offsetRallyPoint;
    public Vector2 offsetedTransformDestination;

    // Nav Mesh and pathfinding
    NavMeshAgent navMeshAgent;

    bool isPathDestinationSet = true;
    
    // Smooth movement variables
    private Vector2 lastFramePosition;
    private Vector2 smoothVelocity;
    private const float VELOCITY_SMOOTHING = 0.15f;
    
    // Pathfinding stability
    private Vector2 lastSetDestination;
    private float lastDestinationUpdateTime = 0f;
    private const float DESTINATION_UPDATE_THRESHOLD = 0.1f; // Minimum distance to update path

    void Start()
    {
        // setup mav mesh and pathfinding
        if ((navMeshAgent = GetComponent<NavMeshAgent>()) != null) {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            // Improve NavMeshAgent smoothness
            navMeshAgent.acceleration = 8f;
            navMeshAgent.angularSpeed = 0f;
            navMeshAgent.stoppingDistance = 0.1f; // Reduce stopping distance for smoother movement
        }
        lastFramePosition = (Vector2)transform.position;
        lastSetDestination = (Vector2)transform.position;
    }

    private void FixedUpdate()
    {
        if (TransformDestination == null) Debug.LogError("why you dont put transform destination??" + gameObject.name);

        if (CurrentMethod == Method.NoMovement) {
            ClearPathDestination();
            return;
        }

        // computation heavy
        //switch (CurrentMethod) {
        //    case Method.SpeedWithTargetAndRange:
        //        CalculateRandomPosition_SpeedWithTargetAndRange();
        //        break;

        //    case Method.SpeedWithFormation:
        //        CalculateRandomPosition_SpeedWithFormation();
        //        break;
        //}

        // Only move when destination is set
        if (!isPathDestinationSet) return;

        switch (CurrentMethod)
        {
            case Method.SpeedWithTarget:
                SetPathDestination((Vector2)TransformDestination.position);
                break;

            case Method.Attacking:
                if(!IsLocked) SetPathDestination((Vector2)TransformDestination.position);
                break;
        }
    }

    private void CalculateRandomPosition_SpeedWithTargetAndRange() {
        if (Vector2.Distance((Vector2)transform.position, (Vector2)navMeshAgent.destination) <= 0.75) {
            Debug.Log("sd");
            if (timer < Time.time) {
                var signX = UnityEngine.Random.Range(0, 2) * 2 - 1;
                var signY = UnityEngine.Random.Range(0, 2) * 2 - 1;
                currentRandomTargetPosition = new Vector2(
                    UnityEngine.Random.Range(navMeshAgent.destination.x + rangeMin * signX, navMeshAgent.destination.x + range * signX),
                    UnityEngine.Random.Range(navMeshAgent.destination.y + rangeMin * signY, navMeshAgent.destination.y + range * signY));
                timer = Time.time + TimeRandomRange;

                SetPathDestination(currentRandomTargetPosition);
            }
        } else {
            currentRandomTargetPosition = (Vector2)navMeshAgent.destination;
        }
    }

    private void CalculateRandomPosition_SpeedWithFormation() {
        offsetedTransformDestination = (Vector2)navMeshAgent.destination + offsetRallyPoint;

        if (Vector2.Distance(transform.position, offsetedTransformDestination) <= range * 1.42f) {
            if (timer < Time.time) {
                var signX = UnityEngine.Random.Range(0, 2) * 2 - 1;
                var signY = UnityEngine.Random.Range(0, 2) * 2 - 1;
                currentRandomTargetPosition = new Vector2(
                    UnityEngine.Random.Range(offsetedTransformDestination.x + rangeMin * signX, offsetedTransformDestination.x + range * signX),
                    UnityEngine.Random.Range(offsetedTransformDestination.y + rangeMin * signY, offsetedTransformDestination.y + range * signY));
                timer = Time.time + TimeRandomRange;
                SetPathDestination(currentRandomTargetPosition);
            }
        } else {
            currentRandomTargetPosition = new Vector2(offsetedTransformDestination.x, offsetedTransformDestination.y);
        }
    }

    public void SetPathDestination(Vector2 newDestination) {
        if (!navMeshAgent) return;
        
        // Only update path if destination has moved significantly
        float distanceToNewDestination = Vector2.Distance(lastSetDestination, newDestination);
        if (distanceToNewDestination < DESTINATION_UPDATE_THRESHOLD && Time.time - lastDestinationUpdateTime < 0.2f) {
            return; // Skip pathfinding update for minor movements
        }
        
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(newDestination);
        lastSetDestination = newDestination;
        lastDestinationUpdateTime = Time.time;
        isPathDestinationSet = true;
    }

    private void ClearPathDestination() {
        if (!navMeshAgent) return;
        navMeshAgent.isStopped = true;
        isPathDestinationSet = false;
        //CurrentMethod = Method.NoMovement;
    }

    // Get smooth velocity for animation
    public Vector2 GetSmoothedVelocity() {
        Vector2 currentPosition = (Vector2)transform.position;
        Vector2 rawVelocity = (currentPosition - lastFramePosition) / Time.deltaTime;
        smoothVelocity = Vector2.Lerp(smoothVelocity, rawVelocity, VELOCITY_SMOOTHING);
        lastFramePosition = currentPosition;
        return smoothVelocity;
    }

    // Updating method when changed in the inspector
    private void OnValidate() {
        if (previousMethod != CurrentMethod) {
            previousMethod = CurrentMethod;
            ClearPathDestination();
            navMeshAgent.isStopped = false;
            isPathDestinationSet = true;
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
