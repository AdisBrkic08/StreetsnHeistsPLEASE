using UnityEngine;
using System.Collections;

public class PlayerShooter2D : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 10f;
    public float fireRate = 0.2f;      // time between shots
    public int maxAmmo = 7;
    public float reloadTime = 1.5f;

    [Header("Recoil")]
    public float recoilForce = 2f;



    [Header("Status (Debug)")]
    public int currentAmmo;
    public bool isReloading = false;
    public bool canShoot = true;       // gets turned off while in car

    private float nextFireTime = 0f;
    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (!canShoot || isReloading) return;

        // Hold left mouse to fire continuously
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        Vector2 shootDirection = GetShootDirection();

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * bulletForce;

        // Recoil knockback
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.AddForce(-shootDirection * recoilForce, ForceMode2D.Impulse);


        currentAmmo--;
    }

    // ----------------------------
    //  AUTO AIM WITH Z-TARGETING
    // ----------------------------
    Vector2 GetShootDirection()
    {
        TargetLockOn lockOn = FindFirstObjectByType<TargetLockOn>();

        // If a target is locked → aim directly at them
        if (lockOn != null && lockOn.GetCurrentTarget() != null)
        {
            return (lockOn.GetCurrentTarget().position - firePoint.position).normalized;
        }

        // Otherwise → normal mouse aim
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return ((Vector2)(mouseWorld - firePoint.position)).normalized;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reloaded!");
    }
}
