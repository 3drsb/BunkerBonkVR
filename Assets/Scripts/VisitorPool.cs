using System.Collections.Generic;
using UnityEngine;

public class VisitorPool : MonoBehaviour
{
    public static VisitorPool Instance;

    [Header("Visitors Tracking")]
    public List<VisitorData2> allVisitors = new List<VisitorData2>();
    public List<VisitorData2> visitedVisitors = new List<VisitorData2>();

    [Header("Bunker Residents")]
    public List<BunkerResident> bunkerResidents = new List<BunkerResident>();

    private void Awake()
    {
        Instance = this;
    }

    // Returns a random visitor who hasn’t visited yet
    public VisitorData2 GetNextVisitor()
    {
        List<VisitorData2> unvisited = allVisitors.FindAll(v => !visitedVisitors.Contains(v));
        if (unvisited.Count == 0)
        {
            Debug.Log("All visitors have appeared at least once.");
            return null;
        }

        VisitorData2 chosen = unvisited[Random.Range(0, unvisited.Count)];
        visitedVisitors.Add(chosen);
        return chosen;
    }

    // Called when the player lets someone in
    public void ActivateBunkerResident(VisitorData2 data)
    {
        foreach (var res in bunkerResidents)
        {
            if (res.visitorName == data.visitorName)
            {
                res.residentObject.SetActive(true);
                res.residentBehavior = res.residentObject.GetComponent<ResidentBehavior>();
                Debug.Log($"{data.visitorName} is now inside the bunker.");
                return;
            }
        }
        Debug.LogWarning($"No bunker resident found for {data.visitorName}");
    }
}

[System.Serializable]
public class BunkerResident
{
    public string visitorName;
    public GameObject residentObject;
    [HideInInspector] public ResidentBehavior residentBehavior;
}
