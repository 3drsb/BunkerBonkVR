using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using System.Linq;


public class RadialSelection : MonoBehaviour
{
    public enum XRButton
    {
        PrimaryButton,
        SecondaryButton,
        GripButton,
        StickClick
    }

    public static RadialSelection Instance { get; private set; }

    [HideInInspector]
    public ResidentInteraction currentTarget;

    private List<string> currentLabels = new List<string>();

    public bool submenuMode = false;

    [Header("Button To Open Radial Menu")]
    public XRButton spawnButton;

    [Header("Radial Settings")]
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public Transform handTransform;
    public float angleBetweenPart = 6f;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    private InputDevice rightHand;
    private bool menuOpen = false;

    private bool triggerPressed = false;
    private bool lastTriggerState = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeRightHand();
        radialPartCanvas.gameObject.SetActive(false);
    }

    void InitializeRightHand()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices
        );

        if (devices.Count > 0)
            rightHand = devices[0];
    }

    void Update()
    {
        if (!rightHand.isValid)
            InitializeRightHand();

        bool openMenuInput = GetButtonState(spawnButton);
        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        // OPEN MENU (only if no menu is open)
        if (openMenuInput && !menuOpen)
        {
            OpenPrimaryMenu();
        }

        // NAVIGATION (while menu is open)
        if (menuOpen)
        {
            GetSelectedRadialPart();
        }

        // CONFIRM using TRIGGER (rising edge)
        if (triggerPressed && !lastTriggerState && menuOpen)
        {
            ConfirmSelection();
        }

        lastTriggerState = triggerPressed;
    }

    bool GetButtonState(XRButton button)
    {
        bool value = false;

        switch (button)
        {
            case XRButton.PrimaryButton:
                rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out value);
                break;
            case XRButton.SecondaryButton:
                rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out value);
                break;
            case XRButton.GripButton:
                rightHand.TryGetFeatureValue(CommonUsages.gripButton, out value);
                break;
            case XRButton.StickClick:
                rightHand.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out value);
                break;
        }
        return value;
    }

    // -------------------------------------------------------
    // OPEN MAIN MENU
    // -------------------------------------------------------

    void OpenPrimaryMenu()
    {
        submenuMode = false;

        currentLabels = new List<string>()
    {
        "Talk",
        "Test",
        "Confirm", // was Give
        "Kill"     // was Leave
    };

        SpawnRadialMenu(currentLabels);

        radialPartCanvas.gameObject.SetActive(true);
        menuOpen = true;
    }


    // -------------------------------------------------------
    // OPEN SUBMENU
    // -------------------------------------------------------

    public void OpenSubmenu(List<string> labels, ResidentInteraction target)
    {
        submenuMode = true;
        currentTarget = target;

        currentLabels = labels;
        SpawnRadialMenu(currentLabels);

        radialPartCanvas.gameObject.SetActive(true);
        menuOpen = true;
    }

    // -------------------------------------------------------
    // BUILD MENU
    // -------------------------------------------------------

    void SpawnRadialMenu(List<string> labels)
    {
        foreach (var obj in spawnedParts)
            Destroy(obj);

        spawnedParts.Clear();

        int count = labels.Count == 0 ? 1 : labels.Count;

        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;

        for (int i = 0; i < count; i++)
        {
            float angle = -i * 360f / count - angleBetweenPart / 2f;

            GameObject part = Instantiate(radialPartPrefab, radialPartCanvas);
            part.transform.localEulerAngles = new Vector3(0, 0, angle);

            Image img = part.GetComponent<Image>();
            if (img != null)
                img.fillAmount = (1f / count) - (angleBetweenPart / 360f);

            TextMeshProUGUI label = part.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = (i < labels.Count) ? labels[i] : "";
            }

            spawnedParts.Add(part);
        }

        currentSelectedRadialPart = -1;
    }

    // -------------------------------------------------------
    // SELECTION
    // -------------------------------------------------------

    void GetSelectedRadialPart()
    {
        if (spawnedParts.Count == 0)
            return;

        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 projected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, projected, -radialPartCanvas.forward);
        if (angle < 0) angle += 360;

        currentSelectedRadialPart = (int)(angle * spawnedParts.Count / 360f);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            Image img = spawnedParts[i].GetComponent<Image>();

            if (i == currentSelectedRadialPart)
            {
                img.color = Color.yellow;
                spawnedParts[i].transform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                img.color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    // -------------------------------------------------------
    // CONFIRM SELECTION
    // -------------------------------------------------------

    void ConfirmSelection()
    {
        // validate
        if (currentSelectedRadialPart < 0 || currentSelectedRadialPart >= currentLabels.Count)
            return;

        if (currentTarget == null)
            return;

        // call into the target first - the target may open a submenu synchronously
        if (submenuMode && currentTarget != null)
        {
            // Confirm submenu selection
            currentTarget.OnSubmenuOptionSelected(currentSelectedRadialPart);
            CloseMenu(); // Close menu after submenu selection
            return;
        }
        else
        {
            // Not in submenu mode => this is a primary selection.
            // Call primary handler. It may synchronously call RadialSelection.Instance.OpenSubmenu(...).
            currentTarget.OnPrimaryOptionSelected(currentSelectedRadialPart);

            // If the earlier call opened a submenu, keep the menu open and let the player select the submenu item.
            if (submenuMode)
            {
                // submenuMode has been set by OpenSubmenu; the menu has been repopulated.
                // do NOT close the menu; keep it open for submenu selection.
                return;
            }
            else
            {
                // No submenu was opened (e.g. Leave) — close menu and clear target.
                CloseMenu();
                return;
            }
        }
    }

    void CloseMenu()
    {
        radialPartCanvas.gameObject.SetActive(false);
        menuOpen = false;
        currentTarget = null;

        // cleanup spawned parts to avoid leftover objects
        foreach (var obj in spawnedParts)
            Destroy(obj);
        spawnedParts.Clear();
        currentLabels.Clear();
        currentSelectedRadialPart = -1;
        submenuMode = false;
    }
}
