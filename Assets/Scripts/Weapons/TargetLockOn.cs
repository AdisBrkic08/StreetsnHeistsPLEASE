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

    void Start()
    {
        if (targetIconPrefab != null)
        {
            currentTargetIcon = Instantiate(targetIconPrefab);
            currentTargetIcon.SetActive(false);
        }
    }

    void Update()
    {
        // AUTO-PATCH: If target is destroyed or out of range, unlock
        if (currentTarget != null)
        {
            if (currentTarget.gameObject == null || Vector3.Distance(transform.position, currentTarget.position) > lockRange + 2f)
            {
                Unlock();
            }
        }

        HandleLockOn();
        UpdateIconPosition();
    }

    public void Unlock()
    {
        currentTarget = null;
        if (currentTargetIcon != null) currentTargetIcon.SetActive(false);
    }

    void HandleLockOn()
    {
        if (Input.GetKeyDown(lockKey))
        {
            if (currentTarget != null) { Unlock(); return; }

            NPCHealth[] npcs = FindObjectsByType<NPCHealth>(FindObjectsSortMode.None);
            var validNpcs = npcs.Where(n => n != null && n.gameObject.activeInHierarchy);
            NPCHealth closest = validNpcs
                .OrderBy(n => Vector3.Distance(transform.position, n.transform.position))
                .FirstOrDefault();

            if (closest != null && Vector3.Distance(transform.position, closest.transform.position) <= lockRange)
            {
                currentTarget = closest.transform;
                if (currentTargetIcon != null) currentTargetIcon.SetActive(true);
            }
        }
    }

    void UpdateIconPosition()
    {
        if (currentTarget == null || currentTargetIcon == null)
        {
            if (currentTargetIcon != null && currentTargetIcon.activeSelf) currentTargetIcon.SetActive(false);
            return;
        }

        currentTargetIcon.transform.position = currentTarget.position + new Vector3(0, 1.6f, 0);
    }

    public Transform GetCurrentTarget() => currentTarget;
}