using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NightCycleManager : MonoBehaviour
{
    public float nightDuration = 360f; // 6 minutes
    public Text timerText;
    public GameObject bedInteraction;

    private float timer;

    void Start()
    {
        StartNight();
    }

    void StartNight()
    {
        timer = nightDuration;
        StartCoroutine(NightTimer());
    }

    IEnumerator NightTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        // Night over, allow bed interaction
        bedInteraction.SetActive(true);
        Debug.Log("Night over! Survived!");
    }
}
