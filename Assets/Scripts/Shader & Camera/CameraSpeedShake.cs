using UnityEngine;

public class CameraSpeedShake : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D carRb; // Assign the car's Rigidbody2D when entering the car

    [Header("Shake Settings")]
    public float maxShake = 0.15f;       // how strong the shake becomes at top speed
    public float shakeSpeed = 25f;       // how fast the shake wiggles
    public float maxCarSpeed = 20f;      // car speed at which shake reaches full intensity

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (carRb == null)
        {
            // No shake when not driving
            transform.localPosition = originalPos;
            return;
        }

        float speed = carRb.linearVelocity.magnitude;

        // Normalized shake amount (0 → 1)
        float shakeAmount = Mathf.Clamp01(speed / maxCarSpeed);

        // Apply shake
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * maxShake * shakeAmount;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed * 0.8f) * maxShake * shakeAmount;

        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
    }

    public void SetCar(Rigidbody2D rb)
    {
        carRb = rb;
    }

    public void ClearCar()
    {
        carRb = null;
    }
}
