using UnityEngine;
using UnityEngine.UI;

public class SpeedbreakerUI : MonoBehaviour
{
    // We no longer need to assign this in the inspector
    private Speedbreaker activeSpeedbreaker;

    public Image fillBar;
    public Image cooldownOverlay;

    void Update()
    {
        // 1. Always try to find the speedbreaker on the car the player is currently in
        UpdateActiveCarReference();

        if (activeSpeedbreaker == null)
        {
            // If not in a car, maybe hide the UI?
            // fillBar.transform.parent.gameObject.SetActive(false); 
            return;
        }

        // 2. Update the visuals
        fillBar.fillAmount = activeSpeedbreaker.EnergyPercent;

        if (activeSpeedbreaker.IsLocked)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = activeSpeedbreaker.CooldownPercent; // Use the actual percent!
        }
        else
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }

    void UpdateActiveCarReference()
    {
        // Find the CarInteraction script that says the player is driving
        CarInteraction[] allCars = Object.FindObjectsByType<CarInteraction>(FindObjectsSortMode.None);

        foreach (CarInteraction car in allCars)
        {
            if (car.isPlayerDriving)
            {
                activeSpeedbreaker = car.GetComponent<Speedbreaker>();
                return;
            }
        }

        activeSpeedbreaker = null; // Player is not in any car
    }
}