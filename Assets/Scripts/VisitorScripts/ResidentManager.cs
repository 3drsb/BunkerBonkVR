using System.Collections.Generic;
using UnityEngine;

public class ResidentManager : MonoBehaviour
{
    public static ResidentManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("All Residents in Bunker")]
    public List<ResidentStatus> residents = new List<ResidentStatus>();

    // -----------------------------
    // RESIDENT TRACKING
    // -----------------------------
    public void RegisterResident(ResidentStatus resident)
    {
        if (!residents.Contains(resident))
            residents.Add(resident);
    }

    public void RemoveResident(ResidentStatus resident)
    {
        if (residents.Contains(resident))
            residents.Remove(resident);
    }

    public List<ResidentStatus> GetAllResidents()
    {
        return residents;
    }

    // -----------------------------
    // ACTIONS
    // -----------------------------
    public string TestResident(ResidentStatus resident, int testIndex)
    {
        if (resident == null) return "No resident selected.";
        return resident.HandleTest(testIndex);
    }

    public string ConfirmResidentHuman(ResidentStatus resident)
    {
        if (resident == null) return "No resident selected.";
        string result = resident.ConfirmHuman();
        if (resident.isConfirmedHuman)
            GameManager.Instance.OnConfirmHuman();
        return result;
    }

    public void KillResident(ResidentStatus resident)
    {
        if (resident == null) return;
        bool wasMonster = resident.isMonster;
        resident.KillResident();
        GameManager.Instance.OnResidentKilled(wasMonster);
        RemoveResident(resident);
    }

    // -----------------------------
    // HELPER
    // -----------------------------
    public List<ResidentStatus> GetAliveHumans()
    {
        return residents.FindAll(r => !r.isDead && !r.isMonster);
    }

    public List<ResidentStatus> GetAliveMonsters()
    {
        return residents.FindAll(r => !r.isDead && r.isMonster);
    }
}
