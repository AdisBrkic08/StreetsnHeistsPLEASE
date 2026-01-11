using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CornerDamageFlash : MonoBehaviour
{
    public Image[] corners;
    public float flashDuration = 0.25f;
    public float maxAlpha = 0.6f;

    void Start()
    {
        // Hide all corners on game start
        foreach (var img in corners)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }

    public void FlashCorners()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float t = 0f;

        // Fade In
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            foreach (var img in corners)
            {
                Color c = img.color;
                c.a = Mathf.Lerp(0, maxAlpha, t);
                img.color = c;
            }
            yield return null;
        }

        // Fade Out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            foreach (var img in corners)
            {
                Color c = img.color;
                c.a = Mathf.Lerp(maxAlpha, 0, t);
                img.color = c;
            }
            yield return null;
        }
    }
}
