using UnityEngine;
using System;

public class WindowButton : MonoBehaviour
{
    public event Action OnPress;

    public void Press()
    {
        OnPress?.Invoke();
    }
}
