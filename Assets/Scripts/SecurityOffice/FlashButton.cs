using UnityEngine;
using System;

public class FlashButton : MonoBehaviour
{
    public event Action OnPress;

    public void Press()
    {
        OnPress?.Invoke();
    }
}
