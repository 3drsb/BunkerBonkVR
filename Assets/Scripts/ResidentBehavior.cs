using UnityEngine;

public class ResidentBehavior : MonoBehaviour
{
    [Header("Resident Info")]
    public string residentName;
    [TextArea] public string[] idleDialogue;
    [TextArea] public string[] repeatDialogue;
    [TextArea] public string[] specialDialogue;

    [Header("Interaction Settings")]
    public float interactionDistance = 2f;
    public Transform playerCamera;

    private bool hasInteracted = false;
    private bool playerNearby = false;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (playerCamera == null) return;

        float dist = Vector3.Distance(playerCamera.position, transform.position);
        playerNearby = dist < interactionDistance;

        if (playerNearby && Input.GetButtonDown("Fire1")) // Replace with your VR trigger input
        {
            Interact();
        }
    }

    public void Interact()
    {
        string chosenLine;

        if (!hasInteracted)
        {
            chosenLine = idleDialogue.Length > 0
                ? idleDialogue[Random.Range(0, idleDialogue.Length)]
                : $"{residentName} looks at you silently.";
            hasInteracted = true;
        }
        else
        {
            chosenLine = repeatDialogue.Length > 0
                ? repeatDialogue[Random.Range(0, repeatDialogue.Length)]
                : $"{residentName} nods again.";
        }

        DialogueManager2.Instance.ShowDialogue($"{residentName}: \"{chosenLine}\"");
    }

    // For story triggers later on
    public void TriggerSpecialDialogue()
    {
        if (specialDialogue.Length > 0)
        {
            string line = specialDialogue[Random.Range(0, specialDialogue.Length)];
            DialogueManager2.Instance.ShowDialogue($"{residentName}: \"{line}\"");
        }
    }
}