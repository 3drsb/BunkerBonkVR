using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR;

public class RadialSelection : MonoBehaviour
{
    // ----------------------
    //  Custom Button Enum
    // ----------------------
    public enum XRButton
    {
        PrimaryButton,        // A or X
        SecondaryButton,      // B or Y
        TriggerButton,        // trigger
        GripButton,           // grip
        StickClick            // thumbstick click
    }

    [Header("Button To Open Radial Menu")]
    public XRButton spawnButton; // <-- NOW CHOOSE IN INSPECTOR!

    [Header("Radial Settings")]
    [Range(2, 10)]
    public int numberOfRadialPart = 4;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10f;
    public Transform handTransform;

    [Header("Events")]
    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    // XR controller
    private InputDevice rightHand;

    // Button tracking
    private bool wasPressedLastFrame = false;


    void Start()
    {
        InitializeRightHand();
    }

    void InitializeRightHand()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
        {
            rightHand = devices[0];
            Debug.Log("Right controller found!");
        }
    }


    void Update()
    {
        if (!rightHand.isValid)
            InitializeRightHand();

        bool isPressed = GetButtonState(spawnButton);

        // GetDown
        if (isPressed && !wasPressedLastFrame)
        {
            SpawnRadialPart();
            radialPartCanvas.gameObject.SetActive(true);
        }

        // Get (held)
        if (isPressed)
        {
            GetSelectedRadialPart();
        }

        // GetUp
        if (!isPressed && wasPressedLastFrame)
        {
            HideAndTriggerSelected();
        }

        wasPressedLastFrame = isPressed;
    }


    // ------------------------------------------
    //  Maps our enum to Unity XR button states
    // ------------------------------------------
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

            case XRButton.TriggerButton:
                rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out value);
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


    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }


    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 projected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, projected, -radialPartCanvas.forward);
        if (angle < 0) angle += 360;

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360f);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            Image img = spawnedParts[i].GetComponent<Image>();

            if (i == currentSelectedRadialPart)
            {
                img.color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                img.color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }


    public void SpawnRadialPart()
    {
        // Position the radial menu at your hand
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;


        foreach (var obj in spawnedParts)
            Destroy(obj);

        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360f / numberOfRadialPart - angleBetweenPart / 2f;
            Vector3 rotation = new Vector3(0, 0, angle);

            GameObject part = Instantiate(radialPartPrefab, radialPartCanvas);
            part.transform.position = radialPartCanvas.position;
            part.transform.localEulerAngles = rotation;

            part.GetComponent<Image>().fillAmount =
                (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

            spawnedParts.Add(part);
        }
    }
}
