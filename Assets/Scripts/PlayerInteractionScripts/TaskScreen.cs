using UnityEngine;
using TMPro;

public class TaskScreen : MonoBehaviour
{
    public TextMeshProUGUI taskText;
    public float taskTime = 5f;

    float progress = 0f;
    bool working = false;

    void Update()
    {
        if (working)
        {
            progress += Time.deltaTime;
            taskText.text = "Processing: " + Mathf.RoundToInt((progress / taskTime) * 100) + "%";

            if (progress >= taskTime)
            {
                working = false;
                taskText.text = "Task Complete!";
            }
        }
    }

    public void BeginTask()
    {
        if (!working)
        {
            working = true;
            progress = 0f;
            taskText.text = "Processing: 0%";
        }
    }
}
