using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    /// <summary>
    /// Call this from any script: Camera.main.GetComponent<CameraShake>().TriggerShake(0.2f, 0.3f);
    /// </summary>
    public void TriggerShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Calculate a random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Apply the offset to the camera's current position
            transform.localPosition += new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
    }
}