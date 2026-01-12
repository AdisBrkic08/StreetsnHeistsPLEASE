using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DamageMotionBlur : MonoBehaviour
{
    public Material blurMaterial;
    [Range(0.1f, 1f)] public float maxBlur = 1f;      // stronger blur
    [Range(0.01f, 0.1f)] public float maxDistort = 0.08f; // stronger distortion
    public float fadeSpeed = 3f;

    float current = 0f;

    public void TriggerDamageBlur()
    {
        current = 1f; // instantly max
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, 0f, Time.deltaTime * fadeSpeed);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (blurMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        blurMaterial.SetFloat("_BlurAmount", current * maxBlur);
        blurMaterial.SetFloat("_Distort", current * maxDistort);

        // Optional: red tint
        blurMaterial.SetColor("_RedTint", new Color(1f, 0f, 0f, 0.4f));

        Graphics.Blit(src, dest, blurMaterial);
    }
}
