using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WaypointMovement))]
public class Player : MonoBehaviour
{
    [SerializeField] private WaypointMovement movement;
    [SerializeField] private float zMovementLevel = 1.0f;

    private void Awake()
    {
        movement = GetComponent<WaypointMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
        {
            Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPosition.z = zMovementLevel;
            movement.AddWaypoint(touchWorldPosition);
        }
    }
}
