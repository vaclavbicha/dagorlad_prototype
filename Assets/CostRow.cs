using UnityEngine;
using UnityEngine.UI;

public class CostRow : MonoBehaviour
{
    [SerializeField]
    UpdateIconText updateIconText;

    [SerializeField]
    Image image;

    [SerializeField]
    Color imageColor;

    public void Start() {
        image = gameObject.GetComponent<Image>();
        imageColor = image.color;
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
        SetBackgroundOpacity(1);
    }

    public void SetUnableToBuy() {
        //gameObject.GetComponent<Button>().interactable = false;
        SetBackgroundOpacity(0.8);
        image.color = new Color(0.24f, 0.24f, 0.24f);
    }

    private void Show() {
        SetBackgroundOpacity(1);
        updateIconText.SetActive(true);
    }

    private void SetBackgroundOpacity(double alpha) {
        gameObject.GetComponent<Image>().color = new Color(imageColor.r, imageColor.g, imageColor.b, (float) alpha);
    }
}
