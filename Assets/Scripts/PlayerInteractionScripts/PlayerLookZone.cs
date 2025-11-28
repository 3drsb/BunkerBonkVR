using UnityEngine;

public class PlayerLookZone : MonoBehaviour
{
    public Transform frontZone;
    public Transform leftZone;
    public Transform rightZone;

    [Header("Detection Settings")]
    public float viewAngle = 35f;

    public enum LookDirection { Front, Left, Right, None }
    public LookDirection currentLookDir = LookDirection.None;

    void Update()
    {
        Vector3 forward = transform.forward;

        if (IsLookingAt(frontZone, forward)) currentLookDir = LookDirection.Front;
        else if (IsLookingAt(leftZone, forward)) currentLookDir = LookDirection.Left;
        else if (IsLookingAt(rightZone, forward)) currentLookDir = LookDirection.Right;
        else currentLookDir = LookDirection.None;
    }

    bool IsLookingAt(Transform target, Vector3 forward)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(forward, dirToTarget);
        return angle < viewAngle;
    }
}
