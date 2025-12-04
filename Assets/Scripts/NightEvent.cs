using UnityEngine;
using System.Collections.Generic;

public class NightEvent : MonoBehaviour
{
    [Tooltip("Chance per monster to kill a human overnight (0-1).")]
    public float killChancePerMonster = 0.15f;

    public void RunNight()
    {
        int monsters = GameManager.Instance.monsterCount;
        int humans = GameManager.Instance.humanCount;

        if (humans <= 0 || monsters <= 0)
        {
            Debug.Log("[Night Event] No deaths tonight.");
            return;
        }

        // Probability: more monsters -> higher chance
        float finalChance = monsters * killChancePerMonster;

        if (Random.value > finalChance)
        {
            Debug.Log("[Night Event] No one died tonight.");
            return;
        }

        // Someone WILL die -> pick random living human
        KillRandomHuman();
    }

    private void KillRandomHuman()
    {
        // Find all residents with ResidentStatus & human & alive
        var all = FindObjectsOfType<ResidentStatus>();
        List<ResidentStatus> humans = new List<ResidentStatus>();

        foreach (var r in all)
            if (!r.isMonster && !r.isDead)
                humans.Add(r);

        if (humans.Count == 0) return;

        ResidentStatus chosen = humans[Random.Range(0, humans.Count)];

        // Kill them without animations
        chosen.isDead = true;
        chosen.gameObject.SetActive(false);

        GameManager.Instance.OnResidentKilled(false);

        DialogueManager2.Instance.ShowDialogue($"{chosen.gameObject.name} was found dead in the morning...");
        Debug.Log($"[Night Event] {chosen.gameObject.name} died overnight.");
    }
}
