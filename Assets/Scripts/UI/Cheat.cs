using UnityEngine;

public class CheatCodeSystem : MonoBehaviour
{
    string currentInput = "";

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            currentInput += c;

            // Limit length so it doesn’t grow forever
            if (currentInput.Length > 30)
                currentInput = currentInput.Substring(currentInput.Length - 30);

            CheckCheats();
        }
    }
    void ExplodeAllCars()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("PoliceVehicle");

        foreach (GameObject car in cars)
        {
            Destroy(car);
        }
    }

    void CheckCheats()
    {
        string input = currentInput.ToLower();

        if (input.Contains("elonmusk"))
        {
            MoneyManager.Instance.AddMoney(999999);
            Debug.Log("💰 Cheat: I didn't know Elon donates to charity. ");
            currentInput = "";
        }

        if (input.Contains("rich"))
        {
            MoneyManager.Instance.AddMoney(10000);
            Debug.Log("💰 Cheat: RICH");
            currentInput = "";
        }

        if (input.Contains("cancer"))
        {
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                player.currentHealth = player.maxHealth;
                Debug.Log("❤️ Cheat: You found the cure for cancer good job!");
            }

            currentInput = "";
        }

        if (input.Contains("flameon"))
        {
            ExplodeAllCars();
            currentInput = "";
        }
    }
}