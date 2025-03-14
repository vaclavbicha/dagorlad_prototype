using TMPro;
using UnityEngine;

public class UpdateIconText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int currentValue = 0;

    public void UpdateDisplay(int value, GameObject sender)
    {
        currentValue = value;
        text.text = value.ToString();
    }

    public void UpdateText(string _text)
    {
        text.text = _text;
    }

    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }
}
