using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public PlayerMoney playerMoney;

    [Header("Prices")]
    public int smgPrice = 300;
    public int akPrice = 600;
    public int shotgunPrice = 500;

    [Header("Buttons")]
    public Button smgButton;
    public Button akButton;
    public Button shotgunButton;

    [Header("Labels")]
    public Text smgText;
    public Text akText;
    public Text shotgunText;

    bool smgBought;
    bool akBought;
    bool shotgunBought;

    void Start()
    {
        if (playerMoney == null)
            playerMoney = FindFirstObjectByType<PlayerMoney>();

        LoadWeapons();
        UpdateUI();
    }

    void Update()
    {
        UpdateButtons();
    }

    void UpdateButtons()
    {
        smgButton.interactable = !smgBought && playerMoney.money >= smgPrice;
        akButton.interactable = !akBought && playerMoney.money >= akPrice;
        shotgunButton.interactable = !shotgunBought && playerMoney.money >= shotgunPrice;
    }

    // ---------------- BUY ----------------

    public void BuySMG()
    {
        if (TryBuy(smgPrice))
        {
            smgBought = true;
            SaveWeapons();
            UpdateUI();
        }
    }

    public void BuyAK()
    {
        if (TryBuy(akPrice))
        {
            akBought = true;
            SaveWeapons();
            UpdateUI();
        }
    }

    public void BuyShotgun()
    {
        if (TryBuy(shotgunPrice))
        {
            shotgunBought = true;
            SaveWeapons();
            UpdateUI();
        }
    }

    // ---------------- CORE ----------------

    bool TryBuy(int price)
    {
        if (playerMoney.money < price)
            return false;

        playerMoney.AddMoney(-price);
        return true;
    }

    // ---------------- SAVE / LOAD ----------------

    void SaveWeapons()
    {
        PlayerPrefs.SetInt("SMG", smgBought ? 1 : 0);
        PlayerPrefs.SetInt("AK", akBought ? 1 : 0);
        PlayerPrefs.SetInt("Shotgun", shotgunBought ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadWeapons()
    {
        smgBought = PlayerPrefs.GetInt("SMG", 0) == 1;
        akBought = PlayerPrefs.GetInt("AK", 0) == 1;
        shotgunBought = PlayerPrefs.GetInt("Shotgun", 0) == 1;
    }

    void UpdateUI()
    {
        if (smgBought) smgText.text = "Owned";
        if (akBought) akText.text = "Owned";
        if (shotgunBought) shotgunText.text = "Owned";
    }
}
