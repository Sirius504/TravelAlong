using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SmoothPath
{
    public int CornerCuttingIterations => cornerCuttingIterations;
    public IReadOnlyList<Vector3> Waypoints => waypoints;

    [SerializeField] private int cornerCuttingIterations = 3;
    private const float cuttingCoeffClose = .25f;
    private const float cuttingCoeffFar = .75f;

    [SerializeField]
    private List<Vector3> waypoints;

    public SmoothPath()
    {
        waypoints = new List<Vector3>();
    }

    public Vector3 this[int index]
    {
        get
        {
            return waypoints[index];
        }
    }

    public void AddWaypoint(Vector3 waypoint)
    {
        waypoints.Add(waypoint);
        if (waypoints.Count > 2)
        {
            var newTail = CutCornersChaikin(waypoints.Skip(Mathf.Max(0, waypoints.Count - 3)).ToList(), CornerCuttingIterations);
            ReplaceTail(waypoints, waypoints.Count - 3, newTail);
        }
    }

    public void RemoveWaypoint(int index)
    {
        waypoints.RemoveAt(index);
    }

    public void MoveWaypoint(int index, Vector3 newPosition)
    {
        waypoints[index] = newPosition;
    }

    public float GetLength()
    {
        float remainingDistance = 0f;
        if (waypoints.Count < 2)
            return remainingDistance;

        for (int i = 0; i < waypoints.Count - 1; i++)
            remainingDistance += (waypoints[i + 1] - waypoints[i]).magnitude;

        return remainingDistance;
    }

    private List<Vector3> CutCornersChaikin(List<Vector3> segment, int iterations = 3)
    {
        if (segment.Count < 3)
            return segment;

        List<Vector3> newPoints = new List<Vector3>();

        int i = 0;
        Vector3 point = Vector3.zero;
        newPoints.Add(segment[i]);
        for (i = 0; i < segment.Count - 1; i++)
        {
            if (i != 0)
            {
                point = segment[i] + (segment[i + 1] - segment[i]) * cuttingCoeffClose;
                newPoints.Add(point);
            }
            if (i != segment.Count - 2)
            {
                point = segment[i] + (segment[i + 1] - segment[i]) * cuttingCoeffFar;
                newPoints.Add(point);
            }
        }
        newPoints.Add(segment[segment.Count - 1]);

        iterations--;
        return iterations == 0 ? newPoints : CutCornersChaikin(newPoints, iterations);
    }

    private void ReplaceTail<T>(List<T> list, int tailStartIndex, List<T> newTail)
    {
        int newListLenght = list.Count - (list.Count - tailStartIndex) + newTail.Count;
        for (int i = tailStartIndex; i < newListLenght; i++)
            if (i < list.Count)
                list[i] = newTail[i - tailStartIndex];
            else
                list.Add(newTail[i - tailStartIndex]);
    }
}

