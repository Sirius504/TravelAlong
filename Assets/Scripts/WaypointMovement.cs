using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    public UnityEvent OnLastWaypointReachedEvent;
    public UnityEvent OnWaypointReachedEvent;

    public SmoothPath Path { get => path; }

    [SerializeField] private SmoothPath path;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float accelerationRate = 1f;
    [SerializeField] private float decelerationRate = 1f;

    private float currentSpeed;

    #region Unity Methods

    private void Start()
    {
        if (path == null)
            path = new SmoothPath();
        path.AddWaypoint(transform.position);
    }

    private void Update()
    {
        currentSpeed = CalculateSpeed(currentSpeed, path.GetLength());
        MoveAlongWaypoints(path, currentSpeed * Time.deltaTime);
    }

    private float CalculateSpeed(float currentSpeed, float remainingDistance)
    {
        if (remainingDistance <= 0f)
            return 0f;

        float decelerationDistance = CalculateDeceleratonDistance(currentSpeed, decelerationRate);
        if (remainingDistance <= decelerationDistance)
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

    private void MoveAlongWaypoints(SmoothPath path, float distance)
    {
        if (path.Waypoints.Count <= 1)
            return;

        int index = 1;
        while (distance > 0f)
        {
            if (index >= path.Waypoints.Count)
                break;

            float nextWaypointDistance = (path.Waypoints[index] - transform.position).magnitude;
            transform.position = Vector3.MoveTowards(transform.position, path.Waypoints[index], distance);
            path.MoveWaypoint(0, transform.position);
            distance -= nextWaypointDistance;

            if (transform.position == path.Waypoints[index])
                OnWaypointReached(index);
            index++;
        }
    }

    private void OnWaypointReached(int index)
    {
        path.RemoveWaypoint(0);
        OnWaypointReachedEvent?.Invoke();
        if (index == path.Waypoints.Count - 1) OnLastWaypointReachedEvent?.Invoke();
    }
}
