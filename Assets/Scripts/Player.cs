using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WaypointMovement))]
public class Player : MonoBehaviour
{
    [SerializeField] private float zMovementLevel = 0.0f;
    private SmoothPath path;

    private void Awake()
    {
        path = GetComponent<WaypointMovement>().Path;
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
        {
            Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPosition.z = zMovementLevel;
            path.AddWaypoint(touchWorldPosition);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickWorldPosition.z = zMovementLevel;
            path.AddWaypoint(clickWorldPosition);
        }
    }
}
