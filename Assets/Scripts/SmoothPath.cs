using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SmoothPath : IEnumerable<Vector3>
{
    public UnityEvent<Vector3> OnWaypointAdded;
    public UnityEvent OnWaypointRemoved;

    public int CornerCuttingIterations => cornerCuttingIterations;
    public int Count => waypoints.Count;


    [SerializeField] private int cornerCuttingIterations = 3;
    private const float cuttingCoeffClose = .25f;
    private const float cuttingCoeffFar = .75f;

    [SerializeField]
    private LinkedList<Vector3> waypoints;


    public SmoothPath()
    {
        waypoints = new LinkedList<Vector3>();
    }

    public void AddWaypoint(Vector3 waypoint)
    {
        waypoints.AddLast(waypoint);
        if (waypoints.Count > 2)
        {
            var newTail = CutCornersChaikin(waypoints.Skip(Mathf.Max(0, waypoints.Count - 3)).ToList(), CornerCuttingIterations);
            ReplaceTail(waypoints.Last.Previous.Previous, new LinkedList<Vector3>(newTail));
        }
        OnWaypointAdded?.Invoke(waypoint);
    }

    public void RemoveFirst()
    {
        waypoints.RemoveFirst();
        OnWaypointRemoved?.Invoke();
    }

    public void MoveFirst(Vector3 newPosition)
    {
        waypoints.First.Value = newPosition;
    }

    public Vector3 CountFromHead(int index)
    {
        if (index > waypoints.Count - 1)
            return CountFromHead(waypoints.Count - 1);
        var pointer = waypoints.First;
        for (int i = 0; i < index; i++)
            pointer = pointer.Next;
        return pointer.Value;
    }

    public float GetLength()
    {
        float remainingDistance = 0f;
        if (waypoints.Count < 2)
            return remainingDistance;

        var pointer = waypoints.First;
        while (pointer.Next != null)
        {
            remainingDistance += (pointer.Next.Value - pointer.Value).magnitude;
            pointer = pointer.Next;
        }

        return remainingDistance;
    }

    private List<Vector3> CutCornersChaikin(List<Vector3> segment, int iterations = 3)
    {
        if (segment.Count < 3)
            return segment;

        List<Vector3> newPoints = new List<Vector3>();

        Vector3 point = Vector3.zero;
        newPoints.Add(segment[0]);
        for (int i = 0; i < segment.Count - 1; i++)
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

    private void ReplaceTail<T>(LinkedListNode<T> tailStart, LinkedList<T> newTail)
    {
        var list = tailStart.List;
                
        while (list.Last != tailStart)
        {
            list.RemoveLast();
        }
        list.RemoveLast();

        while (newTail.Count > 0)
        {
            list.AddLast(newTail.First.Value);
            newTail.RemoveFirst();
        }
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        return waypoints.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return waypoints.GetEnumerator();
    }
}