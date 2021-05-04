using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent OnLastWaypointReachedEvent;
    public UnityEvent OnWaypointReachedEvent;

    [SerializeField] private float speed;

    [SerializeField] private List<Vector3> editorWaypoints;
    [SerializeField] private Queue<Vector3> waypoints;

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

    private void MoveAlongWaypoints(Queue<Vector3> waypoints, float distance)
    {
        if (waypoints.Count <= 0)
            return;
        var currentWaypoint = waypoints.Peek();
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, distance);
        if (transform.position != currentWaypoint)   // haven't reached closest waypoint
            return;

        float overshoot = distance - (transform.position - previousPosition).magnitude;
        bool wasLastWaypointReached = OnWaypointReached(waypoints);
        if (!wasLastWaypointReached && overshoot > 0f)
            MoveAlongWaypoints(waypoints, overshoot);
    }

    private bool OnWaypointReached(Queue<Vector3> waypoints)
    {
        waypoints.Dequeue();
        OnWaypointReachedEvent?.Invoke();
        bool wasLastPointReached = waypoints.Count <= 0;
        if (wasLastPointReached) OnLastWaypointReachedEvent?.Invoke();
        return wasLastPointReached;
    }
}
