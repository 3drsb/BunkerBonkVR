using UnityEngine;
using System.Collections;

public class VisitorBehavior : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float triggerDistance = 2f;

    private VisitorData2 data;
    private Transform doorPoint;
    private bool hasTriggered = false;

    public void Setup(VisitorData2 visitorData, Transform targetPoint)
    {
        data = visitorData;
        doorPoint = targetPoint;
    }

    void Update()
    {
        if (doorPoint == null) return;

        // Move toward the door until close enough
        if (!hasTriggered)
        {
            transform.position = Vector3.MoveTowards(transform.position, doorPoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, doorPoint.position) < triggerDistance)
            {
                hasTriggered = true;
                DialogueManager2.Instance.ShowDialogue($"{data.visitorName}: \"{GetIntroLine()}\"");
            }
        }
    }

    string GetIntroLine()
    {
        string[] pool;

        if (data.isMonster)
        {
            // 70% chance to act human
            bool actHuman = Random.value < 0.7f && data.introHumanComments.Length > 0;
            pool = actHuman ? data.introHumanComments : data.introMonsterComments;
        }
        else
        {
            pool = data.introHumanComments;
        }

        return pool.Length > 0 ? pool[Random.Range(0, pool.Length)] : "...";
    }

    public void ReactToDecision(bool letIn)
    {
        string[] pool;

        if (data.isMonster)
        {
            if (letIn)
            {
                pool = data.letInMonsterComments.Length > 0
                    ? data.letInMonsterComments
                    : data.introMonsterComments;

                StartCoroutine(EnterBunker());
            }
            else
            {
                bool actHuman = Random.value < 0.7f && data.denyHumanComments.Length > 0;
                pool = actHuman ? data.denyHumanComments : data.denyMonsterComments;
                StartCoroutine(LeaveDoor());
            }
        }
        else
        {
            pool = letIn ? data.letInHumanComments : data.denyHumanComments;
            StartCoroutine(letIn ? EnterBunker() : LeaveDoor());
        }

        if (pool == null || pool.Length == 0)
            pool = new[] { "..." };

        string chosen = pool[Random.Range(0, pool.Length)];
        DialogueManager2.Instance.ShowDialogue($"{data.visitorName}: \"{chosen}\"");
    }

    private IEnumerator EnterBunker()
    {
        // optional small delay before removal
        yield return new WaitForSeconds(1f);

        DialogueManager2.Instance.HideDialogue();
        VisitorPool.Instance.ActivateBunkerResident(data);
        VisitorManager2.Instance.RemoveVisitor();
    }


    private IEnumerator LeaveDoor()
    {
        Vector3 away = transform.position + transform.forward * -5f;

        while (Vector3.Distance(transform.position, away) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, away, moveSpeed * Time.deltaTime);
            yield return null;
        }

        DialogueManager2.Instance.HideDialogue();
        VisitorManager2.Instance.RemoveVisitor();
    }
}
