using UnityEngine;
using System.Collections;

public class PlayerMoney : MonoBehaviour
{
    public int money = 0;

    bool isAnimating = false;

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        if (!isAnimating)
            StartCoroutine(AnimateSpend(amount));

        return true;
    }

    IEnumerator AnimateSpend(int amount)
    {
        isAnimating = true;

        int target = money - amount;

        while (money > target)
        {
            money--;
            yield return new WaitForSeconds(0.01f); // Speed of animation
        }

        money = target;

        isAnimating = false;
    }

    // Optional instant add
    public void AddMoney(int amount)
    {
        money += amount;
    }
}
