using TMPro;
using UnityEngine;

public class WaypointMovementDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI generalText;
    [SerializeField] private TextMeshProUGUI waypointsText;
    [SerializeField] private WaypointMovement movement;

    private float movementStartTime;
    private float lastFrameSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    string append = "";

    // Update is called once per frame
    void Update()
    {
        //var movementData = movement.MovementData;
        //if (lastFrameSpeed == 0f && movementData.GetSpeed(movement.CurrentTime) > 0)
            //movementStartTime = Time.time;
        //if (movementData.GetSpeed(movement.CurrentTime) == 4.0f && append == "")
            //append = $"Full speed reached in {Time.time - movementStartTime:F4} sec.";
        //if (lastFrameSpeed > 0f && movementData.GetSpeed(movement.CurrentTime) == 0f)
            //append = "";
        //generalText.text = $"Calculation of distance passed: {movementData.GetDistance(movement.CurrentTime):F4},\t Distance actually passed: {movement.ActuallyPassed:F4}\n"
            //+$"Calculation of speed: {movementData.GetSpeed(movement.CurrentTime):F4},\t Last frame actual delta: {movement.LastFrameDelta / Time.deltaTime:F4}, \t Calculated between frames: {(movementData.GetDistance(movement.CurrentTime) - movementData.GetDistance(movement.CurrentTime - Time.deltaTime))/Time.deltaTime:F4}\n"
            //+$"Time: {movement.CurrentTime:F4}\t" + append;
        //lastFrameSpeed = movementData.GetSpeed(movement.CurrentTime);
    }
}
