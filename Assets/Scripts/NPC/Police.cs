//using UnityEngine;
//using UnityEngine.AI;

//public class PoliceHumanAI : MonoBehaviour
//{
//    private NavMeshAgent agent;
//    private Transform player;

//    [Header("Space Settings")]
//    public float stopDistance = 5f;    // Don't get closer than this
//    public float retreatDistance = 3f; // Back away if player charges at you

//    void Start()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        // These settings are vital for 2D NavMesh
//        agent.updateRotation = false;
//        agent.updateUpAxis = false;

//        GameObject p = GameObject.FindGameObjectWithTag("Player");
//        if (p != null) player = p.transform;
//    }

//    void Update()
//    {
//        if (player == null) return;

//        float dist = Vector2.Distance(transform.position, player.position);

//        if (dist < retreatDistance)
//        {
//            // PLAYER IS TOO CLOSE! Back up.
//            Vector3 dirAway = (transform.position - player.position).normalized;
//            agent.isStopped = false;
//            agent.SetDestination(transform.position + dirAway * 2f);
//        }
//        else if (dist < stopDistance)
//        {
//            // PERFECT RANGE. Stop and aim.
//            agent.isStopped = true;
//            agent.velocity = Vector3.zero;
//        }
//        else
//        {
//            // TOO FAR. Chase player.
//            agent.isStopped = false;
//            agent.SetDestination(player.position);
//        }
//    }
//}