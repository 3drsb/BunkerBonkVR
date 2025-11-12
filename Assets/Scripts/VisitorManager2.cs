using UnityEngine;
using System.Collections;

public class VisitorManager2 : MonoBehaviour
{
    public static VisitorManager2 Instance;

    [Header("Spawn Setup")]
    public Transform spawnPoint;
    public Transform doorPoint;
    public GameObject visitorPrefab;

    private GameObject currentVisitor;
    private VisitorData2 currentVisitorData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnVisitor();
    }

    public void SpawnVisitor()
    {
        if (currentVisitor != null)
            Destroy(currentVisitor);

        // Get a new visitor from the pool
        VisitorData2 nextVisitor = VisitorPool.Instance.GetNextVisitor();
        if (nextVisitor == null)
        {
            Debug.Log("No more visitors to spawn!");
            return;
        }

        currentVisitorData = nextVisitor;
        currentVisitor = Instantiate(visitorPrefab, spawnPoint.position, spawnPoint.rotation);
        VisitorBehavior visitorScript = currentVisitor.GetComponent<VisitorBehavior>();
        visitorScript.Setup(currentVisitorData, doorPoint);
    }

    public void RemoveVisitor()
    {
        if (currentVisitor != null)
        {
            Destroy(currentVisitor);
            currentVisitor = null;
        }
    }

    // Called by DoorDecision2 when player lets them in
    public void LetVisitorIn()
    {
        if (currentVisitorData != null)
        {
            VisitorPool.Instance.ActivateBunkerResident(currentVisitorData);
        }
    }
}
