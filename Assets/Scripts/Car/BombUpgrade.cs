using UnityEngine;

public class CarBombUpgrade : MonoBehaviour
{
    public bool hasCarBomb = false;

    public void UnlockBomb()
    {
        hasCarBomb = true;
        Debug.Log("🔓 Car bomb unlocked!");
    }
}
