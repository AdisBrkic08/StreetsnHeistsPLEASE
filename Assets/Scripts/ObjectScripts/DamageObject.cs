using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 10;
    public int carDamageAmount = 25;
    public bool destroyOnHit = true;
    public GameObject bullet;

    // This stops the bullet from killing the person who shot it
    [HideInInspector] public GameObject owner;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet tag: " + bullet.tag);
        // 1. If we hit the person who fired this, do nothing
        if (other.gameObject == owner) return;

        // 2. If we hit the Player
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damageAmount);
                if (destroyOnHit) Destroy(gameObject);
            }
        }
        // 3. If we hit an NPC
        else if (other.CompareTag("NPC") && bullet.CompareTag("PlayerBullet"))
        {
            NPCHealth nh = other.GetComponent<NPCHealth>();
            if (nh != null)
            {
                // ONLY damage NPCs if the player shot them 
                // (Prevents police from shooting each other)
                if (owner != null && owner.CompareTag("Player"))
                {
                    nh.TakeDamage(damageAmount);
                }

                if (destroyOnHit) Destroy(gameObject);
            }
        }
        // 4. Hit a wall
        else if (other.gameObject.isStatic)
        {
            if (destroyOnHit) Destroy(gameObject);
        }

        // 5. Hit a player vehicle
        else if (other.gameObject.CompareTag("PlayerVehicle"))
        {
            CarHealth ch = other.GetComponent<CarHealth>();
            ch.TakeDamage(carDamageAmount);
        }
    }
}