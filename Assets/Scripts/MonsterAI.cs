using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public enum Path { Front, Left, Right }
    public Path chosenPath;

    public Transform[] pathPoints; // 0 = far, 1 = mid, 2 = door
    public float moveDelay = 8f;   // time between state steps

    public PlayerLookZone playerLook;
    int currentStage = 0;
    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= moveDelay)
        {
            TryMoveForward();
            timer = 0f;
        }

        // If monster reaches last point...
        if (currentStage >= pathPoints.Length - 1)
        {
            CheckForPlayerLook();
        }
    }

    void TryMoveForward()
    {
        // If player is looking down the path, monster freezes.
        if (IsPlayerWatchingThisPath())
            return;

        // Move to next stage
        currentStage = Mathf.Clamp(currentStage + 1, 0, pathPoints.Length - 1);
        transform.position = pathPoints[currentStage].position;
    }

    bool IsPlayerWatchingThisPath()
    {
        return (chosenPath == Path.Front && playerLook.currentLookDir == PlayerLookZone.LookDirection.Front)
            || (chosenPath == Path.Left && playerLook.currentLookDir == PlayerLookZone.LookDirection.Left)
            || (chosenPath == Path.Right && playerLook.currentLookDir == PlayerLookZone.LookDirection.Right);
    }

    void CheckForPlayerLook()
    {
        if (!IsPlayerWatchingThisPath())
        {
            // Player NOT looking — jumpscare!
            Debug.Log("MONSTER ATTACK!");
            // Trigger your jumpscare event here
        }
    }
}
