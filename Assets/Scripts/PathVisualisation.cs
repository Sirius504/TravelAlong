using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualisation : MonoBehaviour
{
    [SerializeField] private WaypointMovement waypointMovement;
    [SerializeField] private LineRenderer lineRenderer;

    private SmoothPath path;

    private void Start()
    {
        if (lineRenderer != null)
            lineRenderer = GetComponent<LineRenderer>();
        path = waypointMovement.Path;
    }

    private void Update()
    {        
        lineRenderer.positionCount = path.Count;
        int i = 0;
        foreach(var waypoint in path)
        {
            lineRenderer.SetPosition(i++, waypoint);
        }
    }
}
