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
                
        while (distance > 0f && path.Count > 1)
        {
            var targetWaypoint = path.CountFromHead(1);
            float nextWaypointDistance = (targetWaypoint - transform.position).magnitude;
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, distance);
            distance -= nextWaypointDistance;

            if (transform.position == targetWaypoint)            
                path.RemoveFirst();            

            path.MoveFirst(transform.position);
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
