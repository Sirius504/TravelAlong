using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    public UnityEvent OnLastWaypointReachedEvent;
    public UnityEvent OnWaypointReachedEvent;

    public IReadOnlyCollection<Vector3> Waypoints { get => waypoints; }

    [SerializeField] private int cornerCuttingIterations = 3;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float accelerationRate = 1f;
    [SerializeField] private float decelerationRate = 1f;

    private const float cuttingCoeffClose = .25f;
    private const float cuttingCoeffFar = .75f;

    private LinkedList<Vector3> waypoints;
    private float currentSpeed;

    #region Unity Methods

    private void Start()
    {
        waypoints = new LinkedList<Vector3>();
    }

    private void Update()
    {
        currentSpeed = CalculateSpeed(currentSpeed, GetRemainingDistance());
        MoveAlongWaypoints(waypoints, currentSpeed * Time.deltaTime);
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
        waypoints.AddLast(waypoint);
        CutCorners(cornerCuttingIterations);
    }

    private void CutCorners(int iterations = 3)
    {
        if (waypoints.Count < 2)
            return;

        waypoints.AddFirst(transform.position);
        var startPoint = waypoints.Last.Previous.Previous;
        for (int j = 0; j < iterations; j++)
        {
            LinkedList<Vector3> newPoints = new LinkedList<Vector3>();
            var currentPoint = startPoint;
            var point = currentPoint.Value + (currentPoint.Next.Value - currentPoint.Value) * cuttingCoeffFar;
            newPoints.AddLast(point);
            currentPoint = currentPoint.Next;
            while (currentPoint != waypoints.Last.Previous)
            {
                point = currentPoint.Value + (currentPoint.Next.Value - currentPoint.Value) * cuttingCoeffClose;
                newPoints.AddLast(point);
                point = currentPoint.Value + (currentPoint.Next.Value - currentPoint.Value) * cuttingCoeffFar;
                newPoints.AddLast(point);
                currentPoint = currentPoint.Next;
            }
            point = currentPoint.Value + (currentPoint.Next.Value - currentPoint.Value) * cuttingCoeffClose;
            newPoints.AddLast(point);
            newPoints.AddLast(waypoints.Last.Value);

            ReplaceTailMultiple(startPoint, newPoints);
        }
        waypoints.RemoveFirst();
    }

    private void ReplaceTailMultiple<T>(LinkedListNode<T> after, LinkedList<T> newTail)
    {
        var originalList = after.List;
        while (after.Next != null)
            originalList.Remove(after.Next);
        foreach (var newElement in newTail)
            originalList.AddLast(newElement);        
    }

    private void MoveAlongWaypoints(LinkedList<Vector3> waypoints, float distance)
    {
        if (waypoints.Count <= 0)
            return;

        var currentWaypoint = waypoints.First.Value;
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

        var currentWaypoint = waypoints.First.Value;
        remainingDistance += (currentWaypoint - transform.position).magnitude;
        foreach (var waypoint in waypoints.Skip(1))
        {
            remainingDistance += (waypoint - currentWaypoint).magnitude;
            currentWaypoint = waypoint;
        }
        return remainingDistance;
    }

    private void OnWaypointReached(LinkedList<Vector3> waypoints, out bool lastWaypointReached)
    {
        waypoints.RemoveFirst();
        OnWaypointReachedEvent?.Invoke();
        lastWaypointReached = waypoints.Count <= 0;
        if (lastWaypointReached) OnLastWaypointReachedEvent?.Invoke();
    }
}
