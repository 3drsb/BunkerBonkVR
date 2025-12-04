using UnityEngine;
using TMPro;

public class ProgressUI : MonoBehaviour
{
    public TextMeshProUGUI progressText;

    void Start()
    {
        if (progressText == null)
            Debug.LogError("ProgressText not assigned!");
    }

    void Update()
    {
        // Update text every frame (could optimize with events)
        progressText.text = $"Confirmed Humans: {GameManager.Instance.confirmedHumans} / {GameManager.Instance.totalAccepted}";
    }
}
