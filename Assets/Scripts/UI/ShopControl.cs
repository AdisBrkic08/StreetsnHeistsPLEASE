using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    [Header("Prices")]
    public int smgPrice = 300;
    public int shotgunPrice = 500;
    public int akPrice = 600;

    [Header("Buttons")]
    public Button smgButton;
    public Button shotgunButton;
    public Button akButton;

    [Header("Labels")]
    public Text smgText;
    public Text shotgunText;
    public Text akText;

    private bool smgBought;
    private bool shotgunBought;
    private bool akBought;

    void Start()
    {
        LoadWeapons();
        UpdateUI();
        UpdateButtons();
    }

    // ---------------- BUY FUNCTIONS ----------------

    public void BuySMG()
    {
        if (smgBought) return;

        if (MoneyManager.Instance.SpendMoney(smgPrice))
        {
            smgBought = true;
            SaveWeapons();
            UpdateUI();
            UpdateButtons();
        }
    }

    public void BuyShotgun()
    {
        if (shotgunBought) return;

        if (MoneyManager.Instance.SpendMoney(shotgunPrice))
        {
            shotgunBought = true;
            SaveWeapons();
            UpdateUI();
            UpdateButtons();
        }
    }

    public void BuyAK()
    {
        if (akBought) return;

        if (MoneyManager.Instance.SpendMoney(akPrice))
        {
            akBought = true;
            SaveWeapons();
            UpdateUI();
            UpdateButtons();
        }
    }

    // ---------------- SAVE / LOAD ----------------

    void SaveWeapons()
    {
        PlayerPrefs.SetInt("SMG", smgBought ? 1 : 0);
        PlayerPrefs.SetInt("Shotgun", shotgunBought ? 1 : 0);
        PlayerPrefs.SetInt("AK", akBought ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadWeapons()
    {
        smgBought = PlayerPrefs.GetInt("SMG", 0) == 1;
        shotgunBought = PlayerPrefs.GetInt("Shotgun", 0) == 1;
        akBought = PlayerPrefs.GetInt("AK", 0) == 1;
    }

    // ---------------- UI ----------------

    void UpdateUI()
    {
        if (smgBought) smgText.text = "Owned";
        if (shotgunBought) shotgunText.text = "Owned";
        if (akBought) akText.text = "Owned";
    }

    void UpdateButtons()
    {
        smgButton.interactable =
            !smgBought && MoneyManager.Instance.money >= smgPrice;

        shotgunButton.interactable =
            !shotgunBought && MoneyManager.Instance.money >= shotgunPrice;

        akButton.interactable =
            !akBought && MoneyManager.Instance.money >= akPrice;
    }
}