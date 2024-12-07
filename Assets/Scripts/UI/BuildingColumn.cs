using UnityEngine;
using UnityEngine.UI;

public class BuildingColumn : MonoBehaviour {
    public UpdateIconText[] array;
    public string currentItemName = "default";
    Image infoImage = null;

    [SerializeField]
    public BuildingColumnMode buildingColumnMode;

    public enum BuildingColumnMode {
        normal,
        utility
    }

    Transform infoToggle;
    Toggle infoButtonToggle;


    public void Start()
    {
        infoImage = transform.parent.GetComponentInParent<BuildingWindow>().infoPanel.GetComponent<Image>();
        infoToggle = transform.Find("Info_Wrap").Find("Toggle_Button").transform;
        infoButtonToggle = infoToggle.GetComponent<Toggle>();
    }
    public void OnBuy()
    {
        UIManager.Instance.OnItemBuy(currentItemName);
    }
    public void OnInfoToggle(Toggle change)
    {
        if (!infoImage) Debug.Log("CANNOT FIND INFO IMAGE");

        if (change.isOn) {
            infoImage.gameObject.SetActive(true);
            infoImage.sprite = GameManager.Instance.infoSprites.Find(sprite => sprite.name == currentItemName);
            UIManager.Instance.lastSelectedInfoToggle = change;
        } else {
            infoImage.gameObject.SetActive(false);
            UIManager.Instance.lastSelectedInfoToggle = null;
        }
    }

    public void TurnOffInfoIcon() {
        infoButtonToggle.isOn = false;
    }
}
