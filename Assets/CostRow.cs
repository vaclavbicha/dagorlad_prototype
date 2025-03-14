using UnityEngine;
using UnityEngine.UI;

public class CostRow : MonoBehaviour
{
    [SerializeField]
    UpdateIconText updateIconText;

    [SerializeField]
    Image image;

    public void Start() {
        image = gameObject.GetComponent<Image>();
    }

    public void Deactivate() {
        Hide();
        updateIconText.UpdateDisplay(0, gameObject);
    }

    public void Hide() {
        image.enabled = false;
        updateIconText.SetActive(false);
    }

    public void SetActive(int costValue) {
        Show();
        updateIconText.UpdateDisplay(costValue, gameObject);
    }

    private void Show() {
        image.enabled = true;
        updateIconText.SetActive(true);
    }
}
