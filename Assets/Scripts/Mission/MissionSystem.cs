using UnityEngine;
using TMPro;
using System.Collections;

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance;

    [Header("UI")]
    public TextMeshProUGUI missionTitleText;
    public TextMeshProUGUI missionDescText;

    [Header("Timing")]
    public float displayTime = 5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartMission(string title, string description)
    {
        if (missionTitleText == null || missionDescText == null)
        {
            Debug.LogError("Mission UI not assigned!");
            return;
        }

        StopAllCoroutines();

        missionTitleText.text = title;
        missionDescText.text = description;

        missionTitleText.gameObject.SetActive(true);
        missionDescText.gameObject.SetActive(true);

        StartCoroutine(HideMissionText());
    }

    IEnumerator HideMissionText()
    {
        yield return new WaitForSeconds(displayTime);

        missionTitleText.gameObject.SetActive(false);
        missionDescText.gameObject.SetActive(false);
    }

    public void CompleteMission()
    {
        missionDescText.gameObject.SetActive(true);
        missionDescText.text = "MISSION PASSED";
        StartCoroutine(HideMissionText());
    }
}
