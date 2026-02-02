using UnityEngine;
using System.Linq;

public class TargetLockOn : MonoBehaviour
{
    [Header("Target Settings")]
    public float lockRange = 10f;
    public KeyCode lockKey = KeyCode.Z;

    [Header("Icon")]
    public GameObject targetIconPrefab;

    private GameObject currentTargetIcon;
    private Transform currentTarget;

    Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (targetIconPrefab != null)
        {
            currentTargetIcon = Instantiate(targetIconPrefab);
            currentTargetIcon.SetActive(false);
        }
    }

    void Update()
    {
        HandleLockOn();
        UpdateIconPosition();
    }

    public void Unlock()
    {
        currentTarget = null;

        if (currentTargetIcon != null)
            currentTargetIcon.SetActive(false);
    }


    void HandleLockOn()
    {
        if (Input.GetKeyDown(lockKey))
        {
            // If already locked → unlock and hide icon
            if (currentTarget != null)
            {
                Unlock();
                return;
            }

            // Otherwise lock to nearest NPC
            NPCHealth[] npcs = FindObjectsByType<NPCHealth>(FindObjectsSortMode.None);
            NPCHealth closest = npcs
                .OrderBy(n => Vector3.Distance(transform.position, n.transform.position))
                .FirstOrDefault();

            if (closest != null && Vector3.Distance(transform.position, closest.transform.position) <= lockRange)
            {
                currentTarget = closest.transform;
                currentTargetIcon.SetActive(true);
            }
        }

    }

    void UpdateIconPosition()
    {
        if (currentTarget == null || currentTargetIcon == null)
            return;

        Vector3 pos = currentTarget.position + new Vector3(0, 1.6f, 0); // raise above head
        currentTargetIcon.transform.position = pos;
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}
