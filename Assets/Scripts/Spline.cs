using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spline
{
    public int SegmentsCount => (points.Count - 4) / 3 + 1;
    public int PointsCount => points.Count;

    [SerializeField]
    private List<Vector3> points;

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }        


    public Spline()
    {
        points = new List<Vector3>()
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
        };
    }

    public Spline(Vector3 p0, Vector3 p1)
    {
        points = new List<Vector3>()
        {
            p0,
            (p1 - p0) / 2f,
            (p1 - p0) / 2f,
            p1
        };
    }

    public void AddSegment(Vector3 newAnchor)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + newAnchor) / 2f);
        points.Add(newAnchor);
    }

    public Vector3[] GetSegment(int index)
    {
        return new Vector3[]
        {
            points[index * 3],
            points[index * 3 + 1],
            points[index * 3 + 2],
            points[index * 3 + 3],
        };
    }

    public void MovePoint(int index, Vector3 newPosition)
    {
        points[index] = newPosition;
    }
}
