using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Resident Counts")]
    public int totalAccepted = 0;       // All residents let into the bunker
    public int confirmedHumans = 0;     // Confirmed by tests or player
    public int monsterCount = 0;        // Residents flagged as monster
    public int humanCount = 0;          // Non-monster residents

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called when player lets someone into bunker
    public void OnResidentAccepted(bool isMonster)
    {
        totalAccepted++;
        if (isMonster) monsterCount++;
        else humanCount++;
        UpdateProgressUI();
    }

    // Called when player confirms someone as human
    public void OnConfirmHuman()
    {
        confirmedHumans++;
        UpdateProgressUI();
    }

    // Called when a resident dies (killed or night event)
    public void OnResidentKilled(bool wasMonster)
    {
        if (wasMonster) monsterCount--;
        else humanCount--;

        UpdateProgressUI();
    }

    // Connect this to your UI slider/text if you want
    public void UpdateProgressUI()
    {
        Debug.Log($"[Progress] Confirmed Humans: {confirmedHumans}/{totalAccepted}   (Monsters left: {monsterCount})");
    }
}
