using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    [SerializeField] private float spawnYOffset = -0.5f;
    void OnDisable()
    {
        SavePosition();
    }

    void OnApplicationQuit()
    {
        SavePosition();
    }

    public void SavePosition()
    {
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.Save();
    }

    void Start()
    {
        LoadPosition();
    }

    void LoadPosition()
    {
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");

            // Apply Y offset so player spawns slightly lower
            transform.position = new Vector2(x, y + spawnYOffset);
        }
    }
}