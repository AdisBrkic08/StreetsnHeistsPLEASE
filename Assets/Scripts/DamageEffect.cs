using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public GameObject bloodEffectPrefab;

    public void SpawnBlood(Vector2 position)
    {
        if (bloodEffectPrefab != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
            Destroy(blood, 2f); // auto-cleanup after 2 s
        }
    }
}
