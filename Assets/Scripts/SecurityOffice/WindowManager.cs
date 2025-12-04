using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager instance;
    private WindowController currentClosedWindow;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void CloseWindow(WindowController window)
    {
        // Open previously closed window if it's not this one
        if (currentClosedWindow != null && currentClosedWindow != window)
        {
            currentClosedWindow.OpenWindow();
        }

        currentClosedWindow = window;
        window.CloseWindow();
    }

    public void OpenWindow(WindowController window)
    {
        if (currentClosedWindow == window)
        {
            currentClosedWindow = null;
        }
        window.OpenWindow();
    }
}
