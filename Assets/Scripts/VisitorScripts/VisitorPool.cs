using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Antlr3.Runtime.Tree.TreeWizard;

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
    // Called when the player lets someone in
    public void ActivateBunkerResident(VisitorData2 data)
    {
        foreach (var res in bunkerResidents)
        {
            if (res.visitorName == data.visitorName)
            {
                // Activate the resident gameobject and cache components
                res.residentObject.SetActive(true);
                res.residentBehavior = res.residentObject.GetComponent<ResidentBehavior>();

                // Try to get ResidentStatus on the resident object and set identity
                res.residentStatus = res.residentObject.GetComponent<ResidentStatus>();
                if (res.residentStatus != null)
                {
                    res.residentStatus.isMonster = data.isMonster;
                }
                else
                {
                    Debug.LogWarning($"ResidentStatus missing on resident object for {data.visitorName}. Add ResidentStatus component to the prefab.");
                }

                Debug.Log($"{data.visitorName} is now inside the bunker. isMonster={data.isMonster}");
                GameManager.Instance.OnResidentAccepted(data.isMonster);
                // After setting residentStatus.isMonster
                ResidentManager.Instance.RegisterResident(res.residentStatus);
                return;
            }
        }
        Debug.LogWarning($"No bunker resident found for {data.visitorName}");
    }



    [System.Serializable]
    public class BunkerResident
    {
        public string visitorName;
        public GameObject residentObject;
        [HideInInspector] public ResidentBehavior residentBehavior;
        [HideInInspector] public ResidentStatus residentStatus; // cached for quick access
    }
}