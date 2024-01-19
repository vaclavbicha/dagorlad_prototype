using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Amount amount;
    public Sprite icon;
    public int currentProduction;
    float sceneStartTime;

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
        sceneStartTime = Time.time;
    }
    public void Update()
    {
        if (amount.type == Utility.ResourceTypes.Time)
        {
            AmountUpdateWithText(Mathf.FloorToInt(Time.time - sceneStartTime));
        }
    }
    public void AmountUpdate(int value)
    {
        amount.value += value;
        On_AmountUpdate?.Invoke(amount.value, gameObject);
    }
    public void AmountUpdateWithText(int value)
    {
        if(amount.type != Utility.ResourceTypes.Supply)
        {
            if(amount.type != Utility.ResourceTypes.Time)
            {
                amount.value += value;
                display.UpdateText(amount.value.ToString() + "<color=#C7D3EF>" + "+" + currentProduction.ToString() + "</color>", gameObject);
            }
            else
            {
                amount.value = value;
                display.UpdateText(amount.GetValueText(), gameObject);
            }
        }
        else
        {
            amount.value += value;
            display.UpdateText(Player.Instance.currentSupply.ToString() + "/" + amount.value, gameObject);
        }
    }
}
