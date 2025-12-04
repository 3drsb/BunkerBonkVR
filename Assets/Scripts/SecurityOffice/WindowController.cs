using UnityEngine;

public class WindowController : MonoBehaviour
{
    public Animator animator;
    public bool IsClosed = false;

    [Header("Buttons")]
    public WindowButton toggleButton; // Button that opens/closes the window
    public FlashButton flashButton;   // Button that flashes the light on this window

    [Header("Monster")]
    public MonsterAI targetMonster;

    void Start()
    {
        toggleButton.OnPress += HandleToggle;
        flashButton.OnPress += HandleFlash;
    }

    void HandleToggle()
    {
        if (!IsClosed)
            WindowManager.instance.CloseWindow(this);
        else
            WindowManager.instance.OpenWindow(this);
    }

    void HandleFlash()
    {
        if (targetMonster != null && !targetMonster.isVisible)
        {
            targetMonster.RevealTemporarily();
        }
    }

    public void CloseWindow()
    {
        animator.SetTrigger("close");
        IsClosed = true;
    }

    public void OpenWindow()
    {
        animator.SetTrigger("open");
        IsClosed = false;
    }

    public void OnKnock()
    {
        // Play knock sound / UI alert
        Debug.Log($"{gameObject.name} is being knocked by Monster!");
    }
}
