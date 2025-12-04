using UnityEngine;
using TMPro;
using System.Collections;

public class DayUIManager : MonoBehaviour
{
    public static DayUIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("UI References")]
    public CanvasGroup dayCanvasGroup;
    public TextMeshProUGUI dayText;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float displayDuration = 1.5f;

    private int currentDay = 1;

    public void StartDay()
    {
        StartCoroutine(ShowDayBanner());
    }

    IEnumerator ShowDayBanner()
    {
        dayText.text = $"Day {currentDay}";
        dayCanvasGroup.alpha = 0;
        dayCanvasGroup.gameObject.SetActive(true);

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        dayCanvasGroup.alpha = 1;

        // Wait
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        dayCanvasGroup.alpha = 0;
        dayCanvasGroup.gameObject.SetActive(false);

        currentDay++;
    }
}
