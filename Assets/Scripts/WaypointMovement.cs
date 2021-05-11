using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    public UnityEvent OnLastWaypointReachedEvent;
    public UnityEvent OnWaypointReachedEvent;

    public IReadOnlyCollection<Vector3> Waypoints { get => waypoints; }

    [SerializeField] private float speed = 4f;
    [SerializeField] private float accelerationRate = 1f;
    [SerializeField] private float decelerationRate = 1f;    

    private Queue<Vector3> waypoints;
    private float currentSpeed;

    #region Unity Methods

    private void Start()
    {
        waypoints = new Queue<Vector3>();
    }

    private void Update()
    {
        currentSpeed = CalculateSpeed(currentSpeed, GetRemainingDistance());
        MoveAlongWaypoints(waypoints, currentSpeed * Time.deltaTime);
        Debug.Log($"Remaining distance: {GetRemainingDistance()}, currentSpeed: {currentSpeed}");
    }

    private float CalculateSpeed(float currentSpeed, float remainingDistance)
    {
        if (remainingDistance <= 0f)        
            return 0f;

        float decelerationDistance = CalculateDeceleratonDistance(currentSpeed, decelerationRate);
        if (remainingDistance < decelerationDistance)
            return currentSpeed - decelerationRate * Time.deltaTime;

        if (currentSpeed < speed)
            return currentSpeed + accelerationRate * Time.deltaTime;

        return speed;
    }

    private float CalculateDeceleratonDistance(float initialSpeed, float decelerationRate)
    {
        float time = initialSpeed / decelerationRate;
        return initialSpeed * time - (decelerationRate * time * time) / 2;
    }

    #endregion

    public void AddWaypoint(Vector3 waypoint)
    {
        waypoints.Enqueue(waypoint);
    }

    private void MoveAlongWaypoints(Queue<Vector3> waypoints, float distance)
    {
        if (waypoints.Count <= 0)
            return;

        var currentWaypoint = waypoints.Peek();
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, distance);
        if (transform.position != currentWaypoint)   // haven't reached current waypoint yet
            return;

        OnWaypointReached(waypoints, out bool lastWaypointReached);
        if (lastWaypointReached)
            return;

        float overshoot = distance - (transform.position - previousPosition).magnitude;
        if (overshoot > 0f)
            MoveAlongWaypoints(waypoints, overshoot);
    }

    public float GetRemainingDistance()
    {
        float remainingDistance = 0f;
        if (waypoints.Count == 0)
            return remainingDistance;

        var currentWaypoint = waypoints.Peek();
        remainingDistance += (currentWaypoint - transform.position).magnitude;
        foreach (var waypoint in waypoints.Skip(1))
        {
            remainingDistance += (waypoint - currentWaypoint).magnitude;
            currentWaypoint = waypoint;
        }
        return remainingDistance;
    }

    private void OnWaypointReached(Queue<Vector3> waypoints, out bool lastWaypointReached)
    {
        waypoints.Dequeue();
        OnWaypointReachedEvent?.Invoke();
        lastWaypointReached = waypoints.Count <= 0;
        if (lastWaypointReached) OnLastWaypointReachedEvent?.Invoke();
    }
}
