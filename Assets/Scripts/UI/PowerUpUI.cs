using UnityEngine;
using UnityEngine.UI;

public class SpeedbreakerUI : MonoBehaviour
{
    private Speedbreaker currentSpeedbreaker; // We find this automatically
    public Image fillBar;
    public Image cooldownOverlay;

    void Update()
    {
        // 🔍 DYNAMIC SEARCH: If we don't have a speedbreaker, or the player switched cars
        if (currentSpeedbreaker == null || !currentSpeedbreaker.car.isDriving)
        {
            FindActiveSpeedbreaker();
        }

        if (currentSpeedbreaker == null)
        {
            if (fillBar) fillBar.fillAmount = 0;
            if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(false);
            return;
        }

        // Update the Energy Bar
        fillBar.fillAmount = currentSpeedbreaker.EnergyPercent;

        // Update the Cooldown Overlay
        if (currentSpeedbreaker.IsLocked)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = currentSpeedbreaker.CooldownPercent;
        }
        else
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }

    void FindActiveSpeedbreaker()
    {
        // Find all cars in the scene
        CarInteraction[] allCars = Object.FindObjectsByType<CarInteraction>(FindObjectsSortMode.None);

        foreach (CarInteraction car in allCars)
        {
            // If this is the car the player is in, grab its speedbreaker
            if (car.isPlayerDriving)
            {
                currentSpeedbreaker = car.GetComponent<Speedbreaker>();
                break;
            }
        }
    }
}