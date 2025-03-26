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
        //gameObject.GetComponent<Button>().interactable = true;
    }

    public void SetUnableToBuy() {
        //gameObject.GetComponent<Button>().interactable = false;
    }

    private void Show() {
        SetBackgroundOpacity(1);
        updateIconText.SetActive(true);
    }

    private void SetBackgroundOpacity(int alpha) {
        gameObject.GetComponent<Image>().color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
    }
}
