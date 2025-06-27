using UnityEngine;
using UnityEngine.UI;

public class CostRow : MonoBehaviour
{
    [SerializeField]
    UpdateIconText updateIconText;

    [SerializeField]
    Image image;

    [SerializeField]
    Color activeColor;

    [SerializeField]
    Color inactiveColor;

    public void Start() {
        image = gameObject.GetComponent<Image>();
        activeColor = image.color;
    }

    public void Deactivate() {
        Hide();
    }

    public void Hide() {
        SetBackgroundOpacity(0);
        updateIconText.UpdateDisplay(0, gameObject);
        updateIconText.SetActive(false);
    }

    public void SetActive(int costValue) {
        Show();
        updateIconText.UpdateDisplay(costValue, gameObject);
    }

    public void SetAbleToBuy() {
        image.color = activeColor;
        updateIconText.SetIconTransparency(1);
        updateIconText.SetTextTransparency(1);
    }

    public void SetUnableToBuy() {
        image.color = inactiveColor;
        updateIconText.SetIconTransparency(0.5f);
        updateIconText.SetTextTransparency(0.5f);
    }

    private void Show() {
        SetBackgroundOpacity(1);
        updateIconText.SetActive(true);
    }

    private void SetBackgroundOpacity(float alpha) {
        Color currentColor = image.color;
        gameObject.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }
}
