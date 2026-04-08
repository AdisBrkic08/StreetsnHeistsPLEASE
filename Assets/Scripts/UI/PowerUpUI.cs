using UnityEngine;
using UnityEngine.UI;

public class SpeedbreakerUI : MonoBehaviour
{
    public Speedbreaker speedbreaker;
    public Image fillBar;
    public Image cooldownOverlay;

    void Start()
    {
        if (speedbreaker == null)
            speedbreaker = Object.FindFirstObjectByType<Speedbreaker>();
    }

    void Update()
    {
        if (speedbreaker == null) return;

        fillBar.fillAmount = speedbreaker.EnergyPercent;

        if (speedbreaker.IsLocked)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 0.5f;

        }
        else
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }
}
