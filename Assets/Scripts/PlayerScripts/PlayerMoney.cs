using UnityEngine;
using System.Collections;

public class PlayerMoney : MonoBehaviour
{
    public int money = 500;
    private GameHUD hud;
    private Coroutine smoothUpdateRoutine;

    void Start()
    {
        hud = Object.FindFirstObjectByType<GameHUD>();
        if (hud != null)
            hud.UpdateHUDNow(); // initial sync
    }

    public void AddMoney(int amount)
    {
        if (amount == 0) return;

        int target = money + amount;
        if (target < 0) target = 0;

        // stop previous animation if still running
        if (smoothUpdateRoutine != null)
            StopCoroutine(smoothUpdateRoutine);

        smoothUpdateRoutine = StartCoroutine(SmoothMoneyIncrease(target));
    }

    private IEnumerator SmoothMoneyIncrease(int targetValue)
    {
        int start = money;
        float duration = 0.5f; // animation duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            money = Mathf.RoundToInt(Mathf.Lerp(start, targetValue, elapsed / duration));

            if (hud != null)
                hud.UpdateHUDNow();

            yield return null;
        }

        money = targetValue;
        if (hud != null)
            hud.UpdateHUDNow();
    }
}
