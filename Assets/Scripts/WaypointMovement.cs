using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public SmoothPath Path { get => path; }

    [SerializeField] private SmoothPath path;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float accelerationRate = 1f;
    [SerializeField] private float decelerationRate = 1f;

    private float movementStartTime = -1f;
    private MovementData movementData;

    #region Unity Methods

    private void Start()
    {
        if (path == null)
            path = new SmoothPath();
        path.OnWaypointAdded.AddListener(OnWaypointAdded);
        path.AddWaypoint(transform.position);
    }

    private void Update()
    {
        if (movementData == null)
            return;
        float time = Time.time - movementStartTime;
        float speed = movementData.GetDistance(time) - movementData.distance + path.GetLength();
        MoveAlongWaypoints(path, speed);
    }

    #endregion

    private void MoveAlongWaypoints(SmoothPath path, float distance)
    {
        if (path.Count <= 1)
            return;

        // path[0] is always at player position and player's next
        // waypoint is at path[1]
        while (distance > 0f && path.Count > 1)
        {
            float nextWaypointDistance = (path[1] - transform.position).magnitude;
            transform.position = Vector3.MoveTowards(transform.position, path[1], distance);
            distance -= nextWaypointDistance;

            if (transform.position == path[1])
                path.RemoveWaypoint(0);

            path.MoveWaypoint(0, transform.position);
        }
    }

    private void OnWaypointAdded(Vector3 waypoint)
    {
        float currentSpeed = movementData == null ? 0f : movementData.GetSpeed(Time.time - movementStartTime);        
        movementData = new MovementData(path.GetLength(),
            currentSpeed,
            speed,
            accelerationRate,
            decelerationRate);
        movementStartTime = Time.time;
    }
}
