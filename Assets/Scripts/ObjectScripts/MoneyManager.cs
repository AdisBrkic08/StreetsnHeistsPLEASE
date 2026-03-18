using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money;


    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveMoney();
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        SaveMoney();
        return true;
    }

    void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();
    }

    void LoadMoney()
    {
        money = PlayerPrefs.GetInt("PlayerMoney", 0);
    }
}