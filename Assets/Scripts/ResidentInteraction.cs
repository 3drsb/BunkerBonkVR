using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(ResidentBehavior))]
public class ResidentInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 2.5f;
    public Transform playerCamera; // optional, defaults to Camera.main
    public GameObject interactUIPrefab; // world-space Canvas prefab with a TextMeshProUGUI

    [Header("UI settings")]
    public Vector3 uiOffset = new Vector3(0f, 1.8f, 0f);
    public float uiFaceLerp = 10f;

    private ResidentBehavior residentBehavior;
    private GameObject activeUI;
    private TextMeshProUGUI uiText;
    private bool playerInRange = false;
    private bool uiShowing = false;

    void Start()
    {
        residentBehavior = GetComponent<ResidentBehavior>();
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (interactUIPrefab == null)
            Debug.LogError($"{name}: interactUIPrefab not assigned. Create a small world-space canvas prefab with a TextMeshProUGUI for the bubble.");
    }

    void Update()
    {
        if (playerCamera == null) return;

        float dist = Vector3.Distance(playerCamera.position, transform.position);
        bool nowInRange = dist <= interactionDistance;

        if (nowInRange && !playerInRange)
        {
            playerInRange = true;
            ShowInteractUI("Interact");
            // register as current radial target so radial selection will route here
            if (RadialSelection.Instance != null)
                RadialSelection.Instance.currentTarget = this;
        }
        else if (!nowInRange && playerInRange)
        {
            playerInRange = false;
            HideInteractUI();
            if (RadialSelection.Instance != null && RadialSelection.Instance.currentTarget == this)
                RadialSelection.Instance.currentTarget = null;
        }

        // If UI is showing, keep it facing the player
        if (uiShowing && activeUI != null)
        {
            // position it above the resident (apply offset in local space)
            Vector3 desiredPos = transform.position + uiOffset;
            activeUI.transform.position = Vector3.Lerp(activeUI.transform.position, desiredPos, Time.deltaTime * 10f);

            // face camera
            Vector3 dir = (playerCamera.position - activeUI.transform.position).normalized;
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            activeUI.transform.rotation = Quaternion.Slerp(activeUI.transform.rotation, look, Time.deltaTime * uiFaceLerp);
        }
    }

    private void ShowInteractUI(string text)
    {
        if (interactUIPrefab == null) return;
        if (activeUI == null)
        {
            activeUI = Instantiate(interactUIPrefab, transform.position + uiOffset, Quaternion.identity);
            uiText = activeUI.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (uiText != null)
            uiText.text = text;

        uiShowing = true;
    }

    private void HideInteractUI()
    {
        if (activeUI != null)
        {
            Destroy(activeUI);
            activeUI = null;
            uiText = null;
        }
        uiShowing = false;
    }

    // Called by RadialSelection when player releases selection
    // optionIndex matches your radial part index (0..n-1)
    public void OnRadialOptionSelected(int optionIndex)
    {
        if (!playerInRange) return; // ignore if player walked away

        // Ask resident for the response
        string response = residentBehavior.GetResponseForOption(optionIndex);
        if (uiText != null)
            uiText.text = response;

        // Optionally do extra behaviour: animations, inventory changes, flags...
        // Example: if optionIndex == 2 (give) you could remove an item from inventory, etc.

        // Reset: after a short delay return to default "Interact" (or keep response until player leaves)
        StartCoroutine(ResetInteractTextAfterDelay(3f));
    }

    private IEnumerator ResetInteractTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerInRange && uiText != null)
            uiText.text = "Interact";
        else
            HideInteractUI();
    }
}
