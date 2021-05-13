using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineBehaviour : MonoBehaviour
{
    public Spline Spline { get; private set; }

    [SerializeField]
    private Spline spline;

    void Start()
    {
        CreateSpline();
    }

    public void CreateSpline()
    {
        Spline = new Spline();
    }
}
