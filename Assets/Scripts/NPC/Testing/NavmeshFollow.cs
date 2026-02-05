using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Removes unwanted rotation
        agent.updateUpAxis = false; // Removes unwanted rotation
    }

    void Update()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        agent.SetDestination(target.position); // Move to target position
    }
}
