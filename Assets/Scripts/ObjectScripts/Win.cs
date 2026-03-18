using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyWinTrigger : MonoBehaviour
{
    [Header("Win Condition")]
    public int requiredMoney = 10000;
    public string winSceneName = "Win";

    [Header("Optional")]
    public bool destroyAfterTrigger = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MoneyManager.Instance == null)
            return;

        if (MoneyManager.Instance.money >= requiredMoney)
        {
            Debug.Log("You beat the game!");
            SceneManager.LoadScene(winSceneName);

            if (destroyAfterTrigger)
                Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not enough money to win!");
        }
    }
}