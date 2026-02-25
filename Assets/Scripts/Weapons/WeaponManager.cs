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

        weapons[currentIndex].SetActive(false);
        currentIndex = newIndex;
        ActivateWeapon(currentIndex);
    }

    void ActivateWeapon(int index)
    {
        weapons[index].SetActive(true);
        UpdateUI();
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