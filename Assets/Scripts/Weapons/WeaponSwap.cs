using UnityEngine;
using UnityEngine.UI;

public class WeaponSwap : MonoBehaviour
{
    [Header("Weapon Objects (logic)")]
    [SerializeField] GameObject[] weapons;

    [Header("Hand Sprites")]
    [SerializeField] Sprite[] handSprites;

    [Header("HUD Icons")]
    [SerializeField] Sprite[] hudIcons;

    [Header("References")]
    [SerializeField] GameObject weaponGripObject;
    [SerializeField] Image weaponIcon;

    SpriteRenderer gripRenderer;

    int currentWeaponIndex = 0;

    void Start()
    {
        gripRenderer = weaponGripObject.GetComponent<SpriteRenderer>();

        UpdateWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(-1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(1);
    }

    void SwitchWeapon(int dir)
    {
        currentWeaponIndex += dir;

        if (currentWeaponIndex < 0)
            currentWeaponIndex = handSprites.Length - 1;

        if (currentWeaponIndex >= handSprites.Length)
            currentWeaponIndex = 0;

        UpdateWeapon();
    }

    void UpdateWeapon()
    {
        // Hand sprite
        if (gripRenderer != null)
            gripRenderer.sprite = handSprites[currentWeaponIndex];

        // HUD icon
        if (weaponIcon != null)
            weaponIcon.sprite = hudIcons[currentWeaponIndex];

        Debug.Log("Current weapon: " + currentWeaponIndex);
    }
}
