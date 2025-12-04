// ResidentStatus.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ResidentBehavior))]
public class ResidentStatus : MonoBehaviour
{
    [Header("Identity")]
    public bool isMonster = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isConfirmedHuman = false;
    [HideInInspector] public bool isSuspected = false;

    private ResidentBehavior behavior;
    private Animator animator; // optional

    [Header("Test settings")]
    [Tooltip("Chance that a monster slips up on a test (0-1). Higher = more obvious.")]
    public float monsterSlipChance = 0.35f;

    private void Awake()
    {
        behavior = GetComponent<ResidentBehavior>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Perform a test on this resident. Returns the text to display.
    /// Also sets isSuspected if the result indicates suspicious behaviour.
    /// </summary>
    public string HandleTest(int testIndex)
    {
        // Bound the index safely
        string baseResponse = behavior.GetTestResponse(testIndex);

        // If already dead, return a short message
        if (isDead) return $"{behavior.residentName} is not responsive.";

        // If a monster: small chance to slip up and return an "inhuman" response
        if (isMonster)
        {
            bool slips = Random.value < monsterSlipChance;
            if (slips)
            {
                isSuspected = true;
                // A quick inhuman/flawed response - keep it short for prototype
                return $"{behavior.residentName}: \"...{GetMonsterGlitchResponse(testIndex)}...\"";
            }
            else
            {
                // acts normal for this test
                return $"{behavior.residentName}: \"{baseResponse}\"";
            }
        }
        else
        {
            // Human - typically gives normal response. Occasionally nervous (flavor).
            if (Random.value < 0.05f) // tiny chance to fumble
            {
                return $"{behavior.residentName}: \"Uh... I think so.\"";
            }
            return $"{behavior.residentName}: \"{baseResponse}\"";
        }
    }

    private string GetMonsterGlitchResponse(int testIndex)
    {
        // Simple variations — keep it short so you can tweak quickly
        switch (testIndex)
        {
            case 0: return "teeth... wrong";
            case 1: return "the eyes... not me";
            case 2: return "ping... echo... nothing";
            case 3: return "gasp... not breathing";
            default: return ".......";
        }
    }

    /// <summary>
    /// Confirm the resident as human (player choice).
    /// </summary>
    public string ConfirmHuman()
    {
        if (isDead) return $"{behavior.residentName} is dead.";

        if (isMonster)
        {
            // player was wrong
            isConfirmedHuman = false;
            isSuspected = true;
            return $"{behavior.residentName} shows signs of being inhuman! You were wrong.";
        }
        else
        {
            isConfirmedHuman = true;
            return $"{behavior.residentName} confirmed human.";
        }
    }

    /// <summary>
    /// Kill the resident (instant kill for prototype). Plays optional animation and deactivates object.
    /// </summary>
    public void KillResident()
    {
        if (isDead) return;

        StartCoroutine(KillSequence());
    }

    private IEnumerator KillSequence()
    {
        isDead = true;

        // Optional: play death animation if animator exists and has "Die" trigger.
        if (animator != null)
        {
            animator.SetTrigger("Die");
            // wait a short moment so animation can start
            yield return new WaitForSeconds(0.8f);
        }
        else
        {
            // small delay so player sees reaction
            yield return new WaitForSeconds(0.4f);
        }

        // Disable interaction UI / components
        var interaction = GetComponent<ResidentInteraction>();
        if (interaction != null)
            interaction.enabled = false;

        // you can swap to a corpse prefab or leave the object disabled
        gameObject.SetActive(false);

        // Quick feedback to the player
        DialogueManager2.Instance.ShowDialogue($"{behavior.residentName} was killed.");
        yield return new WaitForSeconds(1.2f);
        DialogueManager2.Instance.HideDialogue();

        // Note: hook here to update progress, monster counts, score etc. if you have such systems.
    }
}
