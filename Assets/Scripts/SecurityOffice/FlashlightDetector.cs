using UnityEngine;

public class FlashlightDetector : MonoBehaviour
{
    public Transform flashlightTransform;
    public float coneAngle = 30f;
    public float coneDistance = 10f;

    public bool IsMonsterInLight(Transform monster)
    {
        Vector3 direction = monster.position - flashlightTransform.position;
        float angle = Vector3.Angle(flashlightTransform.forward, direction);
        if (angle < coneAngle && direction.magnitude <= coneDistance)
        {
            // Optional: Raycast to ensure no walls blocking light
            if (!Physics.Raycast(flashlightTransform.position, direction.normalized, direction.magnitude, LayerMask.GetMask("Obstacles")))
            {
                return true;
            }
        }
        return false;
    }
}
