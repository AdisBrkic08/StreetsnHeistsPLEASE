using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopPortal : MonoBehaviour
{
    public string shopSceneName = "Shop";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save money before leaving
            PlayerPrefs.Save();

            // Load shop scene
            SceneManager.LoadScene(shopSceneName);
        }
    }
}
