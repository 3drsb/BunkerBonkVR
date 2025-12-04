using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ResidentBehavior))]
public class ResidentInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 2.5f;
    public Transform playerCamera;
    public GameObject interactUIPrefab;

    [Header("UI Settings")]
    public Vector3 uiOffset = new Vector3(0f, 1.8f, 0f);
    public float uiFaceLerp = 12f;

    private ResidentBehavior residentBehavior;
    private GameObject activeUI;
    private TextMeshProUGUI uiText;

    private ResidentStatus residentStatus;
    [HideInInspector] public int currentPrimaryIndexForSubmenu = -1;
    [HideInInspector] public bool interactionLocked = false; // true after confirmed




    private bool playerInRange = false;
    private bool uiShowing = false;

    private bool showingSubmenu = false;
    private int currentPrimaryIndex = -1;

    void Start()
    {
        residentBehavior = GetComponent<ResidentBehavior>();
        if (residentBehavior == null)
            Debug.LogError($"{name}: ResidentBehavior missing!");

        residentStatus = GetComponent<ResidentStatus>();
        if (residentStatus == null)
            Debug.LogWarning($"{name}: ResidentStatus missing.");

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (interactUIPrefab == null)
            Debug.LogError($"{name}: interactUIPrefab not assigned!");
    }


    void Update()
    {
        if (interactionLocked) return; // cannot interact anymore

        if (playerCamera == null) return;

        float dist = Vector3.Distance(playerCamera.position, transform.position);
        bool nowInRange = dist <= interactionDistance;

        // ENTER RANGE
        if (nowInRange && !playerInRange)
        {
            playerInRange = true;
            showingSubmenu = false;
            currentPrimaryIndex = -1;

            ShowInteractUI("Interact");

            if (RadialSelection.Instance != null)
                RadialSelection.Instance.currentTarget = this;
        }
        // EXIT RANGE
        else if (!nowInRange && playerInRange)
        {
            playerInRange = false;
            showingSubmenu = false;
            currentPrimaryIndex = -1;

            HideInteractUI();

            if (RadialSelection.Instance != null &&
                RadialSelection.Instance.currentTarget == this)
            {
                RadialSelection.Instance.currentTarget = null;
            }
        }

        // UI FOLLOW + FACE PLAYER
        if (uiShowing && activeUI != null)
        {
            Vector3 targetPos = transform.position + uiOffset;
            activeUI.transform.position =
                Vector3.Lerp(activeUI.transform.position, targetPos, Time.deltaTime * 10f);

            Vector3 forward = (activeUI.transform.position - playerCamera.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(forward, Vector3.up);
            activeUI.transform.rotation =
                Quaternion.Slerp(activeUI.transform.rotation, lookRot, Time.deltaTime * uiFaceLerp);
        }
    }

    // -------------------------------------------------------------------
    // UI
    // -------------------------------------------------------------------

    private void ShowInteractUI(string text)
    {
        if (interactionLocked)
        {
            text = "Already confirmed";
        }

        if (interactUIPrefab == null) return;

        if (activeUI == null)
        {
            activeUI = Instantiate(interactUIPrefab,
                                   transform.position + uiOffset,
                                   Quaternion.identity);
            uiText = activeUI.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (uiText != null)
            uiText.text = text;

        uiShowing = true;
    }

    private void HideInteractUI()
    {
        if (activeUI != null)
            Destroy(activeUI);

        activeUI = null;
        uiText = null;
        uiShowing = false;
    }

    // -------------------------------------------------------------------
    // RADIAL MENU CALLBACKS
    // -------------------------------------------------------------------

    // PRIMARY: Talk / Test / Give / Leave
    public void OnPrimaryOptionSelected(int optionIndex)
    {
        if (residentBehavior == null)
        {
            Debug.LogError($"{name}: ResidentBehavior is missing!");
            return;
        }

        switch (optionIndex)
        {
            case 0: // TALK
                ShowSubmenu("Talk", residentBehavior.GetTalkOptions());
                currentPrimaryIndex = 0;
                break;

            case 1: // TEST
                ShowSubmenu("Test", residentBehavior.GetTestOptions());
                currentPrimaryIndex = 1;
                break;

            case 2: // CONFIRM
                if (residentStatus != null)
                {
                    string resp = residentStatus.ConfirmHuman();
                    ShowResponse(resp);

                    // Lock future interactions
                    interactionLocked = true;
                    HideInteractUI();

                    GameManager.Instance.OnConfirmHuman(); // update progress
                }
                else
                {
                    ShowResponse("Cannot confirm: ResidentStatus missing!");
                }
                currentPrimaryIndex = -1;
                break;


            case 3: // KILL
                if (residentStatus != null)
                {
                    residentStatus.KillResident();
                }
                else
                {
                    ShowResponse("Cannot kill: ResidentStatus missing!");
                }
                currentPrimaryIndex = -1;
                HideInteractUI();
                if (RadialSelection.Instance != null && RadialSelection.Instance.currentTarget == this)
                    RadialSelection.Instance.currentTarget = null;
                break;
        }
    }



    // SUBMENU OPTION SELECTED
    public void OnSubmenuOptionSelected(int optionIndex)
    {
        if (!showingSubmenu) return;

        if (residentBehavior == null)
        {
            Debug.LogError($"{name}: residentBehavior is null!");
            return;
        }

        string response = "...";

        switch (currentPrimaryIndexForSubmenu)
        {
            case 0: // TALK
                response = residentBehavior.GetTalkResponse(optionIndex);
                break;

            case 1: // TEST
                if (residentStatus != null)
                    response = residentStatus.HandleTest(optionIndex);
                else
                    response = residentBehavior.GetTestResponse(optionIndex);
                break;

            default:
                response = "...";
                break;
        }

        showingSubmenu = false;
        currentPrimaryIndexForSubmenu = -1;

        ShowResponse(response);
    }



    // -------------------------------------------------------------------
    // SUBMENU HANDLING
    // -------------------------------------------------------------------

    private void ShowSubmenu(string title, IEnumerable<string> options)
    {
        showingSubmenu = true;

        if (uiText != null)
            uiText.text = title;

        if (RadialSelection.Instance != null)
            RadialSelection.Instance.OpenSubmenu(new List<string>(options), this);
    }


    private void ShowResponse(string response)
    {
        if (uiText != null)
            uiText.text = response;

        StartCoroutine(ResetInteractTextAfterDelay(3f));
    }

    private IEnumerator ResetInteractTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerInRange && uiText != null)
        {
            uiText.text = "Interact";
        }
        else
        {
            HideInteractUI();
        }
    }
}
