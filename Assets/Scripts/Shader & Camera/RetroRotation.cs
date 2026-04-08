using UnityEngine;

public class RetroRotation : MonoBehaviour
{
    public int rotationSteps = 16; // 16 directions (like a 32-bit top-down game)

    void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.z;
        float stepSize = 360f / rotationSteps;

        // Round the angle to the nearest step
        float snappedAngle = Mathf.Round(currentAngle / stepSize) * stepSize;

        transform.rotation = Quaternion.Euler(0, 0, snappedAngle);
    }
}