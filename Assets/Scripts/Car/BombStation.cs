using UnityEngine;

public class BombSpray : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        SimpleCarController2D car = other.GetComponent<SimpleCarController2D>();
        if (car == null) return;

        CarBomb bomb = car.GetComponent<CarBomb>();
        if (bomb == null) return;

        if (car.enabled)
            bomb.ArmBomb();

        bomb.InstallBomb();
        bomb.ArmBomb(); // ✅ ARM IMMEDIATELY if already driving

        Debug.Log("💣 Bomb ability installed and armed!");
    }
}
