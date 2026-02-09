using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    private float bulletSpeed = 5f;
    private float bulletTime = 0.1f;
    private bool firing = false;

    IEnumerator FireBullet()
    {
        firing = true; 

        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.linearVelocity = bulletSpeed * transform.up;

        yield return new WaitForSeconds(bulletTime);

        firing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && firing == false)
        {
            Debug.Log("FIRE!");
            StartCoroutine(FireBullet());
        }
    }
}
