using UnityEngine;
using System.Collections;

public class PlayerMoney : MonoBehaviour
{
    public int money;

    bool isAnimating = false;

    // ⚠️ TURN THIS ONCE, THEN SET TO false AFTER TESTING
    public bool resetSave = true;

    void Awake()
    {
        // Reset old corrupted save (TEMP)
        if (resetSave)
        {
            PlayerPrefs.DeleteKey("PlayerMoney");
            PlayerPrefs.Save();
            Debug.Log("PLAYER MONEY RESET");
        }

        // Load money
        money = PlayerPrefs.GetInt("PlayerMoney", 0);

        Debug.Log("Loaded money: " + money);

        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        Debug.Log("AddMoney called: +" + amount +
                  " | Before: " + money);

        money += amount;

        Debug.Log("After: " + money);

        SaveMoney();
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        if (!isAnimating)
            StartCoroutine(AnimateSpend(amount));

        return true;
    }

    IEnumerator AnimateSpend(int amount)
    {
        isAnimating = true;

        int target = money - amount;

        while (money > target)
        {
            money--;
            UpdateUI();
            yield return new WaitForSeconds(0.01f);
        }

        money = target;

        SaveMoney();
        UpdateUI();

        isAnimating = false;
    }

    void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();

        Debug.Log("Money saved: " + money);
    }

    void UpdateUI()
    {
        // Example:
        // moneyText.text = "$" + money;
    }
}
