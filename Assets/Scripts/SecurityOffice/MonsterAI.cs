using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public enum MonsterState { Idle, Patrol, ApproachWindow, Knock, Attack }
    public MonsterState currentState;

    [Header("Movement")]
    public NavMeshAgent agent;
    public Transform[] patrolZones; // Zones to roam if no window chosen
    public float teleportDistance = 5f;

    [Header("Attack")]
    public float knockDelay = 2f; // Time player has to close window after knock
    public WindowController targetWindow;
    private bool isAttacking = false;

    [Header("Visibility")]
    public bool isVisible = false;
    public Animator animator;

    void Start()
    {
        currentState = MonsterState.Idle;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case MonsterState.Idle:
                    yield return new WaitForSeconds(1f);
                    currentState = MonsterState.Patrol;
                    break;

                case MonsterState.Patrol:
                    // Walk to a random patrol zone
                    Transform targetZone = patrolZones[Random.Range(0, patrolZones.Length)];
                    agent.SetDestination(targetZone.position);
                    animator.SetBool("isWalking", true);
                    yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);
                    animator.SetBool("isWalking", false);

                    // Small chance to teleport for hybrid movement
                    if (Random.value < 0.3f)
                        TeleportRandomly();

                    // Pick a random open window to approach
                    targetWindow = GetRandomOpenWindow();
                    if (targetWindow != null)
                        currentState = MonsterState.ApproachWindow;

                    break;

                case MonsterState.ApproachWindow:
                    if (targetWindow == null || targetWindow.IsClosed)
                    {
                        currentState = MonsterState.Patrol;
                        break;
                    }

                    agent.SetDestination(targetWindow.transform.position);
                    animator.SetBool("isWalking", true);
                    yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);
                    animator.SetBool("isWalking", false);

                    currentState = MonsterState.Knock;
                    break;

                case MonsterState.Knock:
                    if (targetWindow == null || targetWindow.IsClosed)
                    {
                        currentState = MonsterState.Patrol;
                        break;
                    }

                    animator.SetTrigger("knock");
                    targetWindow.OnKnock();
                    yield return new WaitForSeconds(knockDelay);

                    if (!targetWindow.IsClosed)
                        currentState = MonsterState.Attack;
                    else
                        currentState = MonsterState.Patrol;

                    break;

                case MonsterState.Attack:
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        StartCoroutine(AttackPlayer());
                    }
                    yield return null;
                    break;
            }
            yield return null;
        }
    }

    void TeleportRandomly()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-teleportDistance, teleportDistance), 0, Random.Range(-teleportDistance, teleportDistance));
        agent.Warp(agent.transform.position + randomOffset);
        animator.SetTrigger("teleport");
    }

    IEnumerator AttackPlayer()
    {
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(1.5f); // Attack animation duration
        Debug.Log("Player killed by Monster!");
        isAttacking = false;
        currentState = MonsterState.Idle;
        // TODO: Trigger VR death sequence
    }

    /// <summary>
    /// Temporary reveal for FNAF flash button
    /// </summary>
    public void RevealTemporarily()
    {
        StartCoroutine(RevealCoroutine());
    }

    private IEnumerator RevealCoroutine()
    {
        isVisible = true;
        animator.SetBool("isVisible", true);
        yield return new WaitForSeconds(2f); // Monster visible for 2 seconds
        isVisible = false;
        animator.SetBool("isVisible", false);
    }

    /// <summary>
    /// Get a random open window to target
    /// </summary>
    private WindowController GetRandomOpenWindow()
    {
        WindowController[] allWindows = FindObjectsOfType<WindowController>();
        WindowController[] openWindows = System.Array.FindAll(allWindows, w => !w.IsClosed);

        if (openWindows.Length == 0) return null;

        return openWindows[Random.Range(0, openWindows.Length)];
    }

    void Update()
    {
        // Optional: could add animation for visible/invisible
        animator.SetBool("isVisible", isVisible);
    }
}
