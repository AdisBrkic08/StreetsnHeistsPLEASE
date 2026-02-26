using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public GameObject[] weapons;
    private int currentIndex = 0;

    [Header("HUD Icon Display")]
    [SerializeField] private Image currentWeaponIcon;
    [SerializeField] private Sprite[] hudIcons;

    [Header("Optional Highlight System")]
    [SerializeField] private Image[] weaponSlots; // UI slot backgrounds
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color normalColor = Color.gray;

    void Start()
    {
        // Disable all weapons first (important)
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(false);


        ActivateWeapon(currentIndex);

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(false);

        currentIndex = PlayerPrefs.GetInt("CurrentWeapon", 0);

        if (!IsWeaponOwned(currentIndex))
            currentIndex = GetFirstOwnedWeapon();

        ActivateWeapon(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(2);
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchWeapon(3);

        if (Input.GetButton("Fire1"))
        {
            weapons[currentIndex]
                .GetComponent<PlayerShooter2D>()
                .TryShoot();
        }
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex == currentIndex) return;
        if (newIndex < 0 || newIndex >= weapons.Length) return;
        if (!IsWeaponOwned(newIndex)) return; // Prevent locked weapon use

        weapons[currentIndex].SetActive(false);

        currentIndex = newIndex;

        PlayerPrefs.SetInt("CurrentWeapon", currentIndex);
        PlayerPrefs.Save();

        ActivateWeapon(currentIndex);
    }

    void ActivateWeapon(int index)
    {
        weapons[index].SetActive(true);
        UpdateUI();
    }

    bool IsWeaponOwned(int index)
    {
        switch (index)
        {
            case 0: return true; // Starter pistol always owned
            case 1: return PlayerPrefs.GetInt("SMG", 0) == 1;
            case 2: return PlayerPrefs.GetInt("Shotgun", 0) == 1;
            case 3: return PlayerPrefs.GetInt("AK", 0) == 1;
        }

        return false;
    }

    int GetFirstOwnedWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (IsWeaponOwned(i))
                return i;
        }

        return 0;
    }
    void UpdateUI()
    {
        // Update main icon
        if (currentWeaponIcon != null &&
            hudIcons != null &&
            currentIndex < hudIcons.Length)
        {
            currentWeaponIcon.sprite = hudIcons[currentIndex];
        }

        // Update slot highlight (optional)
        if (weaponSlots != null && weaponSlots.Length > 0)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] != null)
                {
                    weaponSlots[i].color =
                        (i == currentIndex) ? selectedColor : normalColor;
                }
            }
        }
    }
}