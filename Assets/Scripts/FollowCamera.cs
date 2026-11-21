using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 6, -11);
    [SerializeField] float smoothTime = 0.12f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null || !GameManager.Instance.IsRunning) return;

        bool behind = Input.GetKey(KeyCode.C);
        Vector3 currentOffset = offset;
        if (behind) currentOffset.z = -offset.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position + currentOffset,
            ref velocity,
            smoothTime);

        Vector3 focus = target.position + Vector3.forward * (behind ? -14 : 14);
        transform.LookAt(focus);
    }
}
