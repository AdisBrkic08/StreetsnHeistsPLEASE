using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public GameObject shopCanvas;

    public void ExitShop()
    {
        // Hide shop UI
        if (shopCanvas != null)
            shopCanvas.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        Debug.Log("Exited shop");
        SceneManager.LoadScene("MainGame");
    }
    

}
