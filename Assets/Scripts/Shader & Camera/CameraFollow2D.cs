using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public float smoothTime = 0.08f;
    public Transform target;
    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}
