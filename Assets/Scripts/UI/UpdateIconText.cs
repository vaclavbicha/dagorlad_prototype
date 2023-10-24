using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateIconText : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI text;
    public int currentValue = 0;

    public void UpdateDisplay(int value, GameObject sender)
    {
        currentValue = value;
        text.text = value.ToString();
    }
    public void UpdateText(string _text, GameObject sender)
    {
        text.text = _text;
    }
}
