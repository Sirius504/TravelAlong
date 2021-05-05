using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    public UnityEvent OnLastWaypointReachedEvent;
    public UnityEvent OnWaypointReachedEvent;

    [SerializeField] private float speed;
    [SerializeField] private List<Vector3> editorWaypoints;
    private Queue<Vector3> waypoints;

    #region Unity Methods

    private void Start()
    {
        waypoints = new Queue<Vector3>(editorWaypoints);
    }

    void Update()
    {
        MoveAlongWaypoints(waypoints, speed * Time.deltaTime);
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

    private void OnWaypointReached(Queue<Vector3> waypoints, out bool lastWaypointReached)
    {
        waypoints.Dequeue();
        OnWaypointReachedEvent?.Invoke();
        lastWaypointReached = waypoints.Count <= 0;
        if (lastWaypointReached) OnLastWaypointReachedEvent?.Invoke();        
    }
}
