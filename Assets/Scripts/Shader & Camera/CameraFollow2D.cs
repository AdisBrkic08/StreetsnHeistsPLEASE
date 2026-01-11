using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.08f;
    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}
