using System;
using UnityEngine;
public static class Bezier
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
	}

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = GetPoint(p0, p1, p2, t);
        Vector3 b = GetPoint(p1, p2, p3, t);
        return Vector3.Lerp(a, b, t);
    }
}