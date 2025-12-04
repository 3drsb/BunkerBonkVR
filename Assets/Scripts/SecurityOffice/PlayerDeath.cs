using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public CanvasGroup fadeCanvas;
    public float fadeTime = 0.8f;
    public AudioSource audioSource;
    public AudioClip deathClip;

    public void TriggerDeath()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (audioSource != null && deathClip != null)
            audioSource.PlayOneShot(deathClip);

        if (fadeCanvas != null)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Clamp01(t / fadeTime);
                yield return null;
            }
        }

        // For now simply reload the scene or open a "you died" UI
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
