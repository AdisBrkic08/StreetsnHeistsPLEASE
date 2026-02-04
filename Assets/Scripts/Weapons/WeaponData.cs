using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string weaponName;

    [Header("Visual")]
    public Sprite gripSprite;

    [Header("Gun Stats")]
    public int damage = 10;
    public float fireRate = 0.1f;
    public int maxAmmo = 7;
    public float reloadTime = 1.5f;
}
