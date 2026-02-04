using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] Sprite[] weaponSprites;
    [SerializeField] GameObject weaponGripObject;
    SpriteRenderer currentWeaponSprite;

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;

    bool switchDebounce = false;
    void Start()
    {
        currentWeaponSprite = weaponGripObject.GetComponent<SpriteRenderer>();
    }

    void switchWeapon(int number)
    {

        try
        {
            currentWeaponIndex = currentWeaponIndex + number;
            currentWeapon = weapons[currentWeaponIndex];
        }
        catch
        {
            currentWeaponIndex = currentWeaponIndex + -number;
        }

        Debug.Log("current weapon: " + weapons[currentWeaponIndex]);
        Debug.Log("current sprite: " + weaponSprites[currentWeaponIndex]);

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
}