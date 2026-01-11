using UnityEngine;

[ExecuteAlways]
public class CarLights : MonoBehaviour
{
    [Header("Headlight Shape")]
    public float length = 8f;
    public float angle = 45f;
    public int segments = 30;

    [Header("Brightness")]
    public float centerIntensity = 1.0f;
    public float edgeIntensity = 0.05f;
    public Color lightColor = new Color(1f, 0.95f, 0.8f);

    [Header("Toggle Settings")]
    public KeyCode toggleKey = KeyCode.L;
    public bool lightsOn = false;
    public float fadeSpeed = 5f;

    [Header("Driving State (MUST CHANGE FROM CAR SCRIPT!)")]
    public bool isDriving = false;

    float currentCenter;
    float currentEdge;

    Mesh mesh;
    Material mat;

    void OnEnable()
    {
        currentCenter = lightsOn ? centerIntensity : 0f;
        currentEdge = lightsOn ? edgeIntensity : 0f;

        CreateMesh();
        CreateMaterial();
    }

    void Update()
    {
        // Debug message so we know if driving state is wrong
        if (Application.isPlaying)
        {
            if (!isDriving)
                Debug.Log($"[CarLights] Input blocked because isDriving = FALSE on {gameObject.name}");
        }

        HandleToggleInput();
        SmoothFade();

        CreateMesh();
        DrawMesh();
    }

    void HandleToggleInput()
    {
        if (!Application.isPlaying) return;
        if (!isDriving) return;  // <— IMPORTANT

        if (Input.GetKeyDown(toggleKey))
        {
            lightsOn = !lightsOn;
            Debug.Log($"[CarLights] Toggled lights: {lightsOn}");
        }
    }

    void SmoothFade()
    {
        float targetCenter = lightsOn ? centerIntensity : 0f;
        float targetEdge = lightsOn ? edgeIntensity : 0f;

        currentCenter = Mathf.Lerp(currentCenter, targetCenter, Time.deltaTime * fadeSpeed);
        currentEdge = Mathf.Lerp(currentEdge, targetEdge, Time.deltaTime * fadeSpeed);
    }

    void CreateMesh()
    {
        if (mesh == null)
            mesh = new Mesh();

        Vector3[] verts = new Vector3[segments + 2];
        Color[] colors = new Color[verts.Length];
        int[] tris = new int[(segments) * 3];

        verts[0] = Vector3.zero;
        colors[0] = lightColor * currentCenter;

        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float a = Mathf.Lerp(-halfAngle, halfAngle, t);

            verts[i + 1] = new Vector3(
                Mathf.Sin(a) * length,
                Mathf.Cos(a) * length,
                0f
            );

            float blend = Mathf.Lerp(currentCenter, currentEdge, Mathf.Abs(t - 0.5f) * 2f);
            colors[i + 1] = lightColor * blend;
        }

        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            tris[ti++] = 0;
            tris[ti++] = i + 1;
            tris[ti++] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.colors = colors;
        mesh.triangles = tris;
    }

    void CreateMaterial()
    {
        if (mat != null) return;

        Shader shader = Shader.Find("Sprites/Default"); // SAFE shader
        mat = new Material(shader);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    void DrawMesh()
    {
        if (mesh == null || mat == null) return;

        Graphics.DrawMesh(
            mesh,
            transform.localToWorldMatrix,
            mat,
            0
        );
    }
}
