using UnityEngine;
using UnityEngine.UI;

public class HeatUIScript : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite filledStar;  // Assign the solid black star here
    public Sprite emptyStar;   // Assign the white outline star here

    [Header("UI Elements")]
    public Image[] starImages; // Drag your 5 star UI Images here

    [Header("Settings")]
    public float flashSpeed = 4f;

    void Update()
    {
        if (HeatManager.Instance == null) return;

        int currentHeat = HeatManager.Instance.heatLevel;

        for (int i = 0; i < starImages.Length; i++)
        {
            // 1. Stars the player ALREADY HAS (Solid)
            if (i < currentHeat)
            {
                starImages[i].sprite = filledStar;
                starImages[i].color = Color.white; // Ensure no transparency
            }
            // 2. The NEXT star currently being earned (Flashing)
            else if (i == currentHeat && HeatManager.Instance.currentScore > 0)
            {
                starImages[i].sprite = filledStar;

                // Classic GTA Flash: Ping-pong transparency
                float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
                starImages[i].color = new Color(1, 1, 1, alpha);
            }
            // 3. Stars the player DOESN'T HAVE (Outline)
            else
            {
                starImages[i].sprite = emptyStar;
                starImages[i].color = new Color(1, 1, 1, 0.5f); // Slightly faded outline
            }
        }
    }
}