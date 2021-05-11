using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualisation : MonoBehaviour
{
    [SerializeField] private WaypointMovement movement;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform transformOnPath;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {        
        lineRenderer.positionCount = movement.Waypoints.Count + 1;
        lineRenderer.SetPosition(0, transformOnPath.position);
        int i = 1;
        foreach (var waypoint in movement.Waypoints)
            lineRenderer.SetPosition(i++, waypoint);
    }
}
