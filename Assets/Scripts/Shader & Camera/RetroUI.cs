using UnityEngine;
using UnityEngine.UI;

public class RetroUIFlash : MonoBehaviour
{
    private Image img;
    public Color baseColor;
    public float pulseSpeed = 15f;
    public float minIntensity = 0.7f;

    void Start()
    {
        img = GetComponent<Image>();
        baseColor = img.color;
    }

    void Update()
    {
        // Creates a rapid "electronic" flicker
        float noise = Mathf.PerlinNoise(Time.time * pulseSpeed, 0f);
        float lerp = Mathf.Lerp(minIntensity, 1f, noise);

        img.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * lerp);
    }
}