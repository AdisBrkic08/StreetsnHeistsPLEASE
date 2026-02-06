using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] Sprite[] weaponSprites;
    [SerializeField] GameObject weaponGripObject;
    SpriteRenderer currentWeaponSprite;

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;
    bool[] unlocked;

    bool switchDebounce = false;
    void Start()
    {
        currentWeaponSprite = weaponGripObject.GetComponent<SpriteRenderer>();
      
            unlocked = new bool[weapons.Length];

            // Pistol always unlocked
            unlocked[0] = true;

    }

    public void switchWeapon(int number)
    {
        // Move index
        currentWeaponIndex += number;

        // Wrap around
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Length - 1;

        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;

        // Set weapon
        currentWeapon = weapons[currentWeaponIndex];

        Debug.Log("Current weapon: " + weapons[currentWeaponIndex].name);
        Debug.Log("Current sprite: " + weaponSprites[currentWeaponIndex].name);

        // Change grip sprite
        currentWeaponSprite.sprite = weaponSprites[currentWeaponIndex];
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switchWeapon(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            switchWeapon(+1);
        }
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        unlocked[index] = true;

        Debug.Log("Unlocked weapon: " + weapons[index].name);
    }

}