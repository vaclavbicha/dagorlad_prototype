using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Amount amount;
    public Sprite icon;

    public UpdateIconText display;

    public delegate void UpdatedEventDelegate(int value, GameObject sender);
    public event UpdatedEventDelegate On_AmountUpdate;

    private void Awake()
    {
        if (display)
        {
            display.Icon.sprite = icon;
            On_AmountUpdate += display.UpdateDisplay;
        }
    }
    public void AmountUpdate(int value)
    {
        amount.value += value;
        On_AmountUpdate?.Invoke(amount.value, gameObject);
    }

}
