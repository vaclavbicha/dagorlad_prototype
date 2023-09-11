using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateIconText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float Current_Value = 0f;

    public void UpdateCostValue(float value)
    {
        Current_Value = value;
        text.text = value.ToString();
    }
}
