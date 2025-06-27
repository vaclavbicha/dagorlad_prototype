using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateIconText : MonoBehaviour {
    [SerializeField]
    public TextMeshProUGUI text;

    [SerializeField]
    private Image icon;
    public int currentValue = 0;

    public void UpdateDisplay(int value, GameObject sender) {
        currentValue = value;
        text.text = value.ToString();
    }

    public void UpdateText(string _text) {
        text.text = _text;
    }

    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }

    public void SetIconTransparency(float alpha) {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    public void SetTextTransparency(float alpha) {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
