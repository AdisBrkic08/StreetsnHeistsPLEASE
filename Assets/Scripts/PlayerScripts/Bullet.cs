using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 25;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check for NPC
        NPCHealth npc = other.GetComponent<NPCHealth>();
        if (npc != null)
        {
            npc.TakeDamage(damage);
            Destroy(gameObject); // bullet disappears
        }

        // Optionally also hurt player or other things
    }
}
