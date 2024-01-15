using UnityEngine;

public class Takeoff : MonoBehaviour
{
    public float loopDescentSpeed = 0.5f; // Speed for descending during the loop
    public float acceleration = 0.5f;
    public float lateralSpeed = 1.0f;
    public float lateralRange = 3.0f;

    private bool isTakingOff = false;
    private bool isDescending = false; // Flag for descent
    private float totalDistance = 20.0f;
    private float currentDistance = 0.0f;
    private float currentLateralOffset = 0.0f;
    private float currentSpeed = 1.0f;

    private float descentDistance = 3.0f; // Distance to descend
    private bool isDescendingUp = false; // Flag for moving up during descent

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTakingOff = true;
        }
    }

    private void Update()
    {
        if (isTakingOff)
        {
            if (!isDescending)
            {
                // Acceleration
                currentSpeed += acceleration * Time.deltaTime;

                // Lateral movement (oscillation on the x-axis)
                currentLateralOffset = Mathf.Sin(Time.time * lateralSpeed) * lateralRange;

                // Calculate the movement vector
                Vector3 movement = new Vector3(currentLateralOffset, currentSpeed, 0) * Time.deltaTime;

                // Move the object
                transform.Translate(movement);

                // Update distance
                currentDistance += currentSpeed * Time.deltaTime;

                // Check if the desired height is reached
                if (currentDistance >= totalDistance)
                {
                    isDescending = true;
                }
            }
            else
            {
                if (isDescendingUp)
                {
                    // Move up during descent
                    transform.Translate(Vector3.up * loopDescentSpeed * Time.deltaTime);

                    // Update distance for descent
                    currentDistance += loopDescentSpeed * Time.deltaTime;

                    // Check if moved up 1 meter
                    if (currentDistance >= totalDistance + descentDistance)
                    {
                        isDescendingUp = false; // Start moving down
                    }
                }
                else
                {
                    // Move down during descent
                    transform.Translate(Vector3.down * loopDescentSpeed * Time.deltaTime);

                    // Update distance for descent
                    currentDistance -= loopDescentSpeed * Time.deltaTime;

                    // Check if moved down 1 meter
                    if (currentDistance <= totalDistance)
                    {
                        isDescendingUp = true; // Start moving up
                    }
                }
            }
        }
    }
}
