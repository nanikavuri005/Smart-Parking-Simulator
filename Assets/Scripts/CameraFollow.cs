using UnityEngine;

/// <summary>
/// Simple third-person camera that smoothly follows a target and looks at it.
/// Attach to Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Tooltip("The object to follow (player car)")]
    public Transform target;

    [Tooltip("Local-space offset from the target")]
    public Vector3 offset = new Vector3(0f, 5f, -7f);

    [Tooltip("How quickly the camera catches up")]
    public float followSmoothSpeed = 6f;

    [Tooltip("Vertical look-at offset on the target")]
    public float lookAtHeight = 1.2f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Position: smooth move to desired follow point.
        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed * Time.deltaTime);

        // Rotation: keep looking at the car.
        Vector3 lookTarget = target.position + Vector3.up * lookAtHeight;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, followSmoothSpeed * Time.deltaTime);
    }
}
